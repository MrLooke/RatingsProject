using XmlParsing;
using XmlReaders;

if (Directory.Exists("Exports"))
{
    Directory.Delete("Exports", true);
}
Directory.CreateDirectory("Exports");

if (!Directory.Exists("Exports/releases/"))
{
    Directory.CreateDirectory("Exports/releases/");
}

//ReleaseParser.Inspection("./XmlFiles/releases.xml.gz");
ReleaseParser.ReleasesToCsv("./XmlFiles/releases.xml.gz", "Exports/releases/releases");

if (!Directory.Exists("Exports/albums/"))
{
    Directory.CreateDirectory("Exports/albums/");
}

if (!Directory.Exists("Exports/albums_artists/"))
{
    Directory.CreateDirectory("Exports/albums_artists/");
}

string targetDir = "Exports/artists/";
if (!Directory.Exists(targetDir))
{
    Directory.CreateDirectory(targetDir);
}

ArtistsParser.ArtistsToCsv("./XmlFiles/artists.xml", targetDir + "artists");
MastersParsers.AlbumsToCsv("./XmlFiles/masters.xml", "Exports/albums/albums");
MastersParsers.AlbumsToArtistsCsv("./XmlFiles/masters.xml", "Exports/albums_artists/albums_artists");
MastersParsers.AlbumStylesGenresToCsv("./XmlFiles/masters.xml", "Exports/");