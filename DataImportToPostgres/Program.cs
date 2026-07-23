using Microsoft.Extensions.Configuration;
using Npgsql;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing DefaultConnection connection string in appsettings.json.");

string mode = args.Length > 0 ? args[0] : "formats";
switch (mode)
{
    case "full":
        await RunFullImport();
        break;
    case "formats":
        await ImportFormatsIntoAlbumTable();
        break;
    default:
        Console.WriteLine("Usage: dotnet run [full|formats]");
        Console.WriteLine("  full     import all entity and relation tables from XmlParsing exports");
        Console.WriteLine("  formats  backfill the album.format column (default)");
        break;
}

async Task ImportFormatsIntoAlbumTable()
{
    var formatSchema = new Schema("../XmlParsing/Exports/releases/", ["id", "main_id", "format"], ["INT", "INT", "TEXT"]);

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
        await DropTable(connection, tempTable);
    }
}

async Task RunFullImport()
{
    Dictionary<string, Schema> entityTables = new()
    {
        ["artist"] = new Schema("../XmlParsing/Exports/artists/", ["id", "name", "real_name", "description"], ["INT", "TEXT", "TEXT", "TEXT"]),
        ["album"] = new Schema("../XmlParsing/Exports/albums/", ["id", "main_id", "title", "year", "image_url"], ["INT", "INT", "TEXT", "INT", "TEXT"]),
        ["genre"] = new Schema("../XmlParsing/Exports/genres/", ["id", "name"], ["INT", "VARCHAR(50)"]),
        ["music_style"] = new Schema("../XmlParsing/Exports/styles/", ["id", "name"], ["INT", "VARCHAR(50)"]),
    };

    Dictionary<string, Schema> relationTables = new()
    {
        ["album_genre"] = new Schema("../XmlParsing/Exports/albums_genres/", ["album_id", "genre_id"], ["INT", "INT"], ("album_id", "genre_id")),
        ["album_style"] = new Schema("../XmlParsing/Exports/albums_styles/", ["album_id", "style_id"], ["INT", "INT"], ("album_id", "style_id")),
        ["album_artist"] = new Schema("../XmlParsing/Exports/albums_artists/", ["album_id", "artist_id"], ["INT", "INT"], ("album_id", "artist_id"), ("album", "artist")),
    };

    // Entity tables must land before relation tables so foreign keys resolve.
    Console.WriteLine("Importing entity tables...");
    await Task.WhenAll(entityTables.Select(t => ImportTable(connectionString, t.Key, t.Value)));
    Console.WriteLine("Entity table imports complete.");

    Console.WriteLine("Importing relation tables...");
    await Task.WhenAll(relationTables.Select(t => ImportTable(connectionString, t.Key, t.Value)));
    Console.WriteLine("Relation table imports complete.");
}

static async Task ImportTable(string connectionString, string table, Schema schema)
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    string tempTable = "temp_" + table;
    await CreateTempTable(connection, tempTable, schema.GetColumnsWithTypesString());

    await ImportFilesFromFolder(schema.FolderPath, connection, tempTable, schema.GetColumnString());

    if (schema.Relation is null)
    {
        await MoveToEntityTable(connection, tempTable, table);
    }
    else if (schema.ParentTables is null)
    {
        await MoveToRelationTable(connection, tempTable, table, schema.Relation.Value);
    }
    else
    {
        await MoveToRelationTableWithParentCheck(connection, tempTable, table, schema.Relation.Value, schema.ParentTables.Value);
    }
}

static async Task ImportFilesFromFolder(string folderPath, NpgsqlConnection connection, string table, string columnList)
{
    string copyCommand = $"COPY {table}{columnList} FROM STDIN WITH (FORMAT CSV, HEADER true)";

    foreach (string filePath in Directory.GetFiles(folderPath).Order())
    {
        var writer = await connection.BeginTextImportAsync(copyCommand);
        using var fileReader = new StreamReader(filePath);

        try
        {
            string? line;
            while ((line = await fileReader.ReadLineAsync()) != null)
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

static async Task DropTable(NpgsqlConnection connection, string table)
{
    await using var dropCommand = new NpgsqlCommand($"DROP TABLE IF EXISTS {table};", connection);
    await dropCommand.ExecuteNonQueryAsync();
}

static async Task MoveToEntityTable(NpgsqlConnection connection, string tempTable, string actualTable)
{
    try
    {
        await using var command = new NpgsqlCommand(@$"
            INSERT INTO {actualTable}
            SELECT DISTINCT * FROM {tempTable}
            ON CONFLICT (id) DO NOTHING", connection);
        await command.ExecuteNonQueryAsync();
    }
    finally
    {
        await DropTable(connection, tempTable);
    }
}

static async Task MoveToRelationTable(NpgsqlConnection connection, string tempTable, string actualTable, (string Col1, string Col2) relation)
{
    try
    {
        await using var command = new NpgsqlCommand(@$"
            INSERT INTO {actualTable}
            SELECT DISTINCT * FROM {tempTable}
            ON CONFLICT ({relation.Col1}, {relation.Col2}) DO NOTHING", connection);
        await command.ExecuteNonQueryAsync();
    }
    finally
    {
        await DropTable(connection, tempTable);
    }
}

static async Task MoveToRelationTableWithParentCheck(NpgsqlConnection connection, string tempTable, string actualTable, (string Col1, string Col2) relation, (string Parent1, string Parent2) parents)
{
    try
    {
        await using var command = new NpgsqlCommand(@$"
            INSERT INTO {actualTable} ({relation.Col1}, {relation.Col2})
            SELECT DISTINCT t.{relation.Col1}, t.{relation.Col2}
            FROM {tempTable} t
            INNER JOIN {parents.Parent1} p1 ON t.{relation.Col1} = p1.id
            INNER JOIN {parents.Parent2} p2 ON t.{relation.Col2} = p2.id
            ON CONFLICT ({relation.Col1}, {relation.Col2}) DO NOTHING", connection);
        await command.ExecuteNonQueryAsync();
    }
    finally
    {
        await DropTable(connection, tempTable);
    }
}

public record Schema(string FolderPath, List<string> Columns, List<string> Types, (string Col1, string Col2)? Relation = null, (string Parent1, string Parent2)? ParentTables = null)
{
    public string GetColumnString() => $"({string.Join(",", Columns)})";

    public string GetColumnsWithTypesString()
    {
        if (Columns.Count != Types.Count)
        {
            throw new InvalidOperationException($"Schema for {FolderPath} has {Columns.Count} columns but {Types.Count} types.");
        }

        var columnTypes = Columns.Zip(Types, (column, type) => $"{column} {type}");
        return $"({string.Join(",", columnTypes)})";
    }
}
