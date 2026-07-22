using Npgsql;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration config = builder.Build();
string connectionString = config.GetConnectionString("DefaultConnection")!;
await ImportFormatsIntoAlbumTable();

async Task ImportFormatsIntoAlbumTable()
{
    var formatSchema = new Schema(false, "../XmlParsing/Exports/releases/", new List<string>(["id", "main_id", "format"]), new List<string>(["INT", "INT", "TEXT"]));

    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    string tempTable = "temp_format";
    await CreateTempTable(connection, tempTable, formatSchema.GetColumnsWithTypesString());

    await ImportFilesFromFolder(formatSchema.FolderPath, connection, tempTable, formatSchema.GetColumnString());

    try
    {
        await using var updateActualFromTemp = new NpgsqlCommand($@"
            UPDATE album a
            SET format = t.format
            FROM {tempTable} t
            WHERE a.id = t.main_id 
            AND a.format IS NULL;
        ", connection);
        await updateActualFromTemp.ExecuteNonQueryAsync();
    }
    finally
    {
        await using var dropTempCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tempTable};", connection);
        await dropTempCmd.ExecuteNonQueryAsync();
    }
}


async Task MainImport()
{
    Dictionary<string, Schema> entityTableMap = new()
    {
        ["artist"] = new Schema(false, "../XmlParsing/Exports/artists/", new List<string>(["id", "name", "real_name", "description"]), new List<string>(["INT", "TEXT", "TEXT", "TEXT"])),
        ["album"] = new Schema(false, "../XmlParsing/Exports/albums/", new List<string>(["id", "main_id", "title", "year", "image_url"]), new List<string>(["INT", "INT", "TEXT", "INT", "TEXT"])),
        ["genre"] = new Schema(false, "../XmlParsing/Exports/genres/", new List<string>(["id", "name"]), new List<string>(["INT", "VARCHAR(50)"])),
        ["music_style"] = new Schema(false, "../XmlParsing/Exports/styles/", new List<string>(["id", "name"]), new List<string>(["INT", "VARCHAR(50)"])),
    };

    Dictionary<string, Schema> relationTableMap = new()
    {
        ["album_genre"] = new Schema(false, "../XmlParsing/Exports/albums_genres/", new List<string>(["album_id", "genre_id"]), new List<string>(["INT", "INT"]), ("album_id", "genre_id")),
        ["album_style"] = new Schema(false, "../XmlParsing/Exports/albums_styles/", new List<string>(["album_id", "style_id"]), new List<string>(["INT", "INT"]), ("album_id", "style_id")),
        ["album_artist"] = new Schema(false, "../XmlParsing/Exports/albums_artists/", new List<string>(["album_id", "artist_id"]), new List<string>(["INT", "INT"]), ("album_id", "artist_id"), ("album", "artist")),
    };

    var entityTasks = new List<Task>();
    foreach (var (tableName, schema) in entityTableMap)
    {
        if (schema.isLoadingTable && schema.Columns.Count == schema.Types.Count)
        {
            entityTasks.Add(ExecuteImportFromFilesToTable(connectionString, tableName, schema));
        }
    }

    Console.WriteLine("Running entity table import tasks concurrently...");
    await Task.WhenAll(entityTasks);
    Console.WriteLine("Entity table imports complete.");

    var relationTasks = new List<Task>();
    foreach (var (tableName, schema) in relationTableMap)
    {
        if (schema.isLoadingTable && schema.Columns.Count == schema.Types.Count)
        {
            relationTasks.Add(ExecuteImportFromFilesToTable(connectionString, tableName, schema));
        }
    }

    Console.WriteLine("Running relational table import tasks concurrently...");
    await Task.WhenAll(relationTasks);
    Console.WriteLine("Relational table imports complete.");
}

static async Task ExecuteImportFromFilesToTable(string connectionString, string table, Schema schema)
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    string tempTable = "temp_" + table;
    await CreateTempTable(connection, tempTable, schema.GetColumnsWithTypesString());

    await ImportFilesFromFolder(schema.FolderPath, connection, tempTable, schema.GetColumnString());

    if (schema.Relation == null)
    {
        await ExecuteMoveToActualTable(connection, tempTable, table);
    }
    else if(schema.ParentTables == null)
    {
        await ExecuteMoveToRelationTable(connection, tempTable, table, schema.Relation.Value);
    }
    else
    {
        await ExecuteMoveToRelationTableWithParentCheck(connection, tempTable, table, schema.Relation.Value, schema.ParentTables.Value);
    }
}

static async Task ImportFilesFromFolder(string folderPath, NpgsqlConnection connection, string table, string schema)
{
    string[] files = Directory.GetFiles(folderPath);
    string copyCommand = $"COPY {table}{schema} FROM STDIN WITH (FORMAT CSV, HEADER true)";

    foreach (var filePath in files)
    {
        var writer = await connection.BeginTextImportAsync(copyCommand);
        using var filestream = new StreamReader(filePath);

        try
        {
            string? line;
            while ((line = await filestream.ReadLineAsync()) != null)
            {
                await writer.WriteLineAsync(line);
            }
        }
        catch (Exception e)
        {
            await writer.CancelAsync();

            Console.WriteLine($"Error on file {filePath}");
            Console.WriteLine(e.Message);
        }
        finally
        {
            await writer.DisposeAsync();
        }

        Console.WriteLine($"Finished importing {filePath}");
    }

}

static async Task CreateTempTable(NpgsqlConnection connection, string tempTable, string columnTypesString)
{
    await using var createTempCommand = new NpgsqlCommand($"CREATE TEMP TABLE {tempTable} {columnTypesString};", connection);
    await createTempCommand.ExecuteNonQueryAsync();
}


static async Task ExecuteMoveToActualTable(NpgsqlConnection connection, string tempTable, string actualTable)
{
    try
    {
        await using var moveFromTempToActual = new NpgsqlCommand(@$"
            INSERT INTO {actualTable}
            SELECT DISTINCT * FROM {tempTable}
            ON CONFLICT (id) DO NOTHING", connection);
        await moveFromTempToActual.ExecuteNonQueryAsync();
    }
    finally
    {
        await using var dropTempCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tempTable};", connection);
        await dropTempCmd.ExecuteNonQueryAsync();
    }
}

static async Task ExecuteMoveToRelationTable(NpgsqlConnection connection, string tempTable, string actualTable, (string Id1, string Id2) relation)
{
    try
    {
        await using var moveFromTempToRelation = new NpgsqlCommand(@$"
            INSERT INTO {actualTable}
            SELECT DISTINCT * FROM {tempTable}
            ON CONFLICT ({relation.Id1}, {relation.Id2}) DO NOTHING", connection);
        await moveFromTempToRelation.ExecuteNonQueryAsync();
    }
    finally
    {
        await using var dropTempCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tempTable};", connection);
        await dropTempCmd.ExecuteNonQueryAsync();
    }
}

static async Task ExecuteMoveToRelationTableWithParentCheck(NpgsqlConnection connection, string tempTable, string actualTable, (string Col1, string Col2) relation, (string Parent1, string Parent2) parents)
{
    try
    {
        await using var moveFromTempToRelation = new NpgsqlCommand(@$"
            INSERT INTO {actualTable} ({relation.Col1}, {relation.Col2})
            SELECT DISTINCT t.{relation.Col1}, t.{relation.Col2}
            FROM {tempTable} t
            INNER JOIN {parents.Parent1} p1 ON t.{relation.Col1} = p1.id            
            INNER JOIN {parents.Parent2} p2 ON t.{relation.Col2} = p2.id
            ON CONFLICT ({relation.Col1}, {relation.Col2}) DO NOTHING", connection);
        await moveFromTempToRelation.ExecuteNonQueryAsync();
    }
    finally
    {
        await using var dropTempCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tempTable};", connection);
        await dropTempCmd.ExecuteNonQueryAsync();
    }
}


public record Schema(bool isLoadingTable, string FolderPath, List<string> Columns, List<string> Types, (string, string)? Relation = null, (string, string)? ParentTables = null) 
{ 
    public string GetColumnString()
    {
        return $"({string.Join(",", Columns)})";
    }

    public string GetColumnsWithTypesString()
    {
        var columnTypes = Columns.Zip(Types, (column, type) => $"{column} {type}");
        return $"({string.Join(",", columnTypes)})";
    }
}