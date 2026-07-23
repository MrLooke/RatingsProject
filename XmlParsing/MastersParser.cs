using System.IO.Compression;
using System.Text;
using System.Xml;

namespace XmlParsing
{
    internal static class MastersParser
    {
        private static readonly HashSet<string> AlbumSkipTags = ["genres", "styles", "videos"];
        private static readonly HashSet<string> GenreStyleSkipTags = ["videos", "artists"];

        internal static void AlbumsToCsv(string xmlZipPath, string csvFileName)
        {
            using var csv = new BatchedCsvWriter(csvFileName, "Id,MainId,Title,Year,ImageUrl");
            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var xmlReader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "master") continue;

                int id = Helpers.ParseIntOrNull(xmlReader.GetAttribute("id")) ?? 0;
                int mainId = 0;
                int? year = null;
                string title = "";
                bool lowQuality = false;

                using var innerReader = xmlReader.ReadSubtree();
                while (innerReader.Read())
                {
                    while (innerReader.NodeType == XmlNodeType.Element && AlbumSkipTags.Contains(innerReader.Name))
                    {
                        innerReader.Skip();
                    }

                    if (innerReader.NodeType != XmlNodeType.Element) continue;

                    if (innerReader.Depth != 1) continue;

                    switch (innerReader.Name)
                    {
                        case "main_release":
                            mainId = Helpers.ParseIntOrNull(innerReader.ReadString()) ?? 0;
                            break;
                        case "title":
                            title = innerReader.ReadString();
                            break;
                        case "year":
                            year = Helpers.ParseIntOrNull(innerReader.ReadString());
                            break;
                        case "data_quality":
                            lowQuality = innerReader.ReadString() == "Needs Major Changes";
                            break;
                    }
                }

                if (!lowQuality && id != 0 && !string.IsNullOrEmpty(title))
                {
                    // ImageUrl is not present in the masters dump; the column exists to match
                    // the album import schema and is backfilled separately.
                    csv.WriteLine($"{id},{mainId},\"{title.EscapeCsv()}\",{year},\"\"");
                }
            }
        }

        internal static void AlbumsToArtistsCsv(string xmlZipPath, string csvFileName)
        {
            using var csv = new BatchedCsvWriter(csvFileName, "AlbumId,ArtistId");
            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var xmlReader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "master") continue;

                int albumId = Helpers.ParseIntOrNull(xmlReader.GetAttribute("id")) ?? 0;
                var artistIds = new List<int>();

                // In the masters dump, <id> elements only occur inside the <artists> block.
                using var innerReader = xmlReader.ReadSubtree();
                while (innerReader.Read())
                {
                    if (innerReader.NodeType != XmlNodeType.Element || innerReader.Name != "id") continue;

                    int? artistId = Helpers.ParseIntOrNull(innerReader.ReadString());
                    if (artistId.HasValue)
                    {
                        artistIds.Add(artistId.Value);
                    }
                }

                if (albumId == 0) continue;

                foreach (int artistId in artistIds)
                {
                    csv.WriteLine($"{albumId},{artistId}");
                }
            }
        }

        internal static void AlbumStylesGenresToCsv(string xmlZipPath, string targetFolder)
        {
            const string genrePath = "genres/genres";
            const string stylePath = "styles/styles";
            const string albumGenrePath = "albums_genres/album_genres";
            const string albumStylePath = "albums_styles/album_styles";
            CreateDirectories([genrePath, stylePath, albumGenrePath, albumStylePath], targetFolder);

            Dictionary<string, int> genreIds = new();
            Dictionary<string, int> styleIds = new();

            using var genreCsv = new BatchedCsvWriter(targetFolder + albumGenrePath, "AlbumId,GenreId");
            using var styleCsv = new BatchedCsvWriter(targetFolder + albumStylePath, "AlbumId,StyleId");

            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var xmlReader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "master") continue;

                int albumId = Helpers.ParseIntOrNull(xmlReader.GetAttribute("id")) ?? 0;
                string title = "";
                bool lowQuality = false;
                var genres = new List<int>();
                var styles = new List<int>();

                using var innerReader = xmlReader.ReadSubtree();
                while (innerReader.Read())
                {
                    // Skip() leaves the reader on the next node, so keep skipping in place
                    // rather than letting the loop's Read() consume that node unseen.
                    while (innerReader.NodeType == XmlNodeType.Element && GenreStyleSkipTags.Contains(innerReader.Name))
                    {
                        innerReader.Skip();
                    }

                    if (innerReader.NodeType != XmlNodeType.Element) continue;

                    switch (innerReader.Name)
                    {
                        case "title":
                            if (string.IsNullOrEmpty(title))
                            {
                                title = innerReader.ReadString();
                            }
                            break;
                        case "data_quality":
                            lowQuality = innerReader.ReadString() == "Needs Major Changes";
                            break;
                        case "genre":
                            AddLookup(genreIds, innerReader.ReadString(), genres);
                            break;
                        case "style":
                            AddLookup(styleIds, innerReader.ReadString(), styles);
                            break;
                    }
                }

                if (lowQuality || albumId == 0 || string.IsNullOrEmpty(title)) continue;

                foreach (int genreId in genres)
                {
                    genreCsv.WriteLine($"{albumId},{genreId}");
                }

                foreach (int styleId in styles)
                {
                    styleCsv.WriteLine($"{albumId},{styleId}");
                }
            }

            WriteLookupCsv(genreIds, targetFolder + genrePath);
            WriteLookupCsv(styleIds, targetFolder + stylePath);
        }

        private static void AddLookup(Dictionary<string, int> ids, string name, List<int> target)
        {
            if (string.IsNullOrEmpty(name)) return;

            if (!ids.TryGetValue(name, out int id))
            {
                id = ids.Count + 1;
                ids[name] = id;
            }

            target.Add(id);
        }

        private static void WriteLookupCsv(Dictionary<string, int> entries, string path)
        {
            using var writer = new StreamWriter($"{path}.csv", false, Encoding.UTF8);
            writer.WriteLine("Id,Name");
            foreach (var (name, id) in entries)
            {
                writer.WriteLine($"{id},\"{name.EscapeCsv()}\"");
            }
        }

        private static void CreateDirectories(string[] paths, string targetFolder)
        {
            foreach (string path in paths)
            {
                Directory.CreateDirectory(targetFolder + Path.GetDirectoryName(path));
            }
        }
    }
}
