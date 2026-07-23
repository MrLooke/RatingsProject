using System.Xml;
using XmlParsing;

if (Directory.Exists("Exports"))
{
    Directory.Delete("Exports", true);
}

string[] exportDirs = ["Exports/releases", "Exports/artists", "Exports/albums", "Exports/albums_artists"];
foreach (string dir in exportDirs)
{
    Directory.CreateDirectory(dir);
}

Run("Release formats", () => ReleaseParser.ReleasesToCsv("./XmlFiles/releases.xml.gz", "Exports/releases/releases"));
Run("Artists", () => ArtistsParser.ArtistsToCsv("./XmlFiles/artists.xml", "Exports/artists/artists"));
Run("Albums", () => MastersParser.AlbumsToCsv("./XmlFiles/masters.xml", "Exports/albums/albums"));
Run("Album-artist relations", () => MastersParser.AlbumsToArtistsCsv("./XmlFiles/masters.xml", "Exports/albums_artists/albums_artists"));
Run("Genres and styles", () => MastersParser.AlbumStylesGenresToCsv("./XmlFiles/masters.xml", "Exports/"));

static void Run(string step, Action parse)
{
    try
    {
        Console.WriteLine($"{step}: parsing...");
        parse();
        Console.WriteLine($"{step}: complete.");
    }
    catch (XmlException ex)
    {
        Console.WriteLine($"{step}: XML error at line {ex.LineNumber}, position {ex.LinePosition} — {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{step}: failed — {ex.Message}");
    }
}
