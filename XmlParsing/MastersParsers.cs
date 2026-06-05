using System.Text;
using System.Xml;
using XmlParsing;

namespace XmlReaders {

    public static class MastersParsers {
        internal class Album
        {
            public int Id { get; set; }
            public int MainId { get; set; }
            public string Title { get; set; } = "";
            public int ArtistId { get; set; }
            public int? Year { get; set; }
            public string? ImageUrl { get; set; } = "";

            public List<int> Genres { get; set; } = new List<int>();
            public List<int> Styles { get; set; } = new List<int>();
        }

        public static void AlbumsToCsv(string xmlFilePath, string csvFileName)
        {
            int fileNumber = 1;
            int count = 0;
            int batchSize = 100000;
            const string headers = "Id,MainId,Title,Year,ImageUrl";

            StreamWriter csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
            csvWriter.WriteLine(headers);

            using var xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            try
            {
                HashSet<string> skipTags = new HashSet<string>(["genres", "styles", "videos"]);
                while (xmlReader.Read())
                {
                    Album album = new Album();
                    bool abort = false;
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "master")
                    {
                        Helpers.ParseIntAndSet(xmlReader.GetAttribute("id") ?? "", (val) => album.Id = val);

                        using var innerReader = xmlReader.ReadSubtree();
                        while (innerReader.Read())
                        {
                            if (innerReader.NodeType == XmlNodeType.Element)
                            {
                                if (skipTags.Contains(innerReader.Name))
                                {
                                    innerReader.Skip();
                                    continue;
                                }

                                if (innerReader.Depth == 1)
                                {
                                    switch (innerReader.Name)
                                    {
                                        case "main_release":
                                            Helpers.ParseIntAndSet(innerReader.ReadString(), (val) => album.MainId = val);
                                            break;
                                        case "title":
                                            album.Title = innerReader.ReadString().Replace("\"", "\"\"");
                                            break;
                                        case "year":
                                            Helpers.ParseIntAndSet(innerReader.ReadString(), (val) => album.Year = val);
                                            break;
                                        case "data_quality":
                                            abort = innerReader.ReadString() == "Needs Major Changes";
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (!abort && album.Id != 0 && !string.IsNullOrEmpty(album.Title))
                    {
                        string safeTitle = album.Title?.Replace("\"", "\"\"") ?? "";
                        string safeUrl = album.ImageUrl?.Replace("\"", "\"\"") ?? "";

                        csvWriter.WriteLine($"{album.Id},{album.MainId},\"{safeTitle}\",{album.Year},\"{safeUrl}\"");
                        count++;

                        if (count == batchSize)
                        {
                            csvWriter.Dispose();

                            fileNumber++;
                            csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
                            csvWriter.WriteLine(headers);
                            count = 0;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"\nFATAL XML ERROR: {ex.Message}");
                Console.WriteLine($"Failed near Line: {ex.LineNumber}, Position: {ex.LinePosition}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUNEXPECTED ERROR: {ex.Message}");
            }
            finally
            {
                if (csvWriter != null)
                {
                    csvWriter.Flush();
                    csvWriter.Dispose();
                    Console.WriteLine("AlbumWriter safely closed.");
                }
            }
        }

        public static void AlbumsToArtistsCsv(string xmlFilePath, string csvFileName)
        {
            int fileNumber = 1;
            int count = 0;
            int batchSize = 100000;
            HashSet<string> styleSet = new HashSet<string>();
            const string headers = "AlbumId,ArtistId";

            StreamWriter csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
            csvWriter.WriteLine(headers);

            using var xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            try
            {
                HashSet<string> skipTags = new HashSet<string>(["genres", "styles", "videos"]);
                while (xmlReader.Read())
                {
                    int albumId = 0;
                    List<int> artistIds = new();

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "master")
                    {
                        Helpers.ParseIntAndSet(xmlReader.GetAttribute("id") ?? "", (val) => albumId = val);

                        using var innerReader = xmlReader.ReadSubtree();
                        while (innerReader.Read())
                        {
                            if (innerReader.NodeType == XmlNodeType.Element && innerReader.Name == "id")
                            {
                                try
                                {
                                    artistIds.Add(innerReader.ReadElementContentAsInt());
                                }
                                catch
                                {
                                    Console.WriteLine("Invalid ID within tag.");
                                }
                            }
                        }
                    }

                    if (albumId != 0 && artistIds.Count > 0)
                    {
                        foreach (int artist in artistIds)
                        {
                            csvWriter.WriteLine($"{albumId},{artist}");
                            count++;
                        }

                        if (count >= batchSize)
                        {
                            csvWriter.Dispose();

                            fileNumber++;
                            csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
                            csvWriter.WriteLine(headers);
                            count = 0;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"\nFATAL XML ERROR: {ex.Message}");
                Console.WriteLine($"Failed near Line: {ex.LineNumber}, Position: {ex.LinePosition}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUNEXPECTED ERROR: {ex.Message}");
            }
            finally
            {
                if (csvWriter != null)
                {
                    csvWriter.Flush();
                    csvWriter.Dispose();
                    Console.WriteLine("AlbumArtistWriter safely closed.");
                }
            }
        }

        public static void AlbumStylesGenresToCsv(string xmlFilePath, string targetFolder)
        {
            const string albumGenreHeaders = "AlbumId,GenreId";
            const string albumStyleHeaders = "AlbumId,StyleId";

            const string genrePath = "genres/genres";
            const string stylePath = "styles/styles";
            const string albumGenrePath = "albums_genres/album_genres";
            const string albumStylePath = "albums_styles/album_styles";
            CreatePaths([genrePath, stylePath, albumGenrePath, albumStylePath], targetFolder);

            int batchSize = 100000;
            int genreCount = 0, genreFileNumber = 0;
            int styleCount = 0, styleFileNumber = 0;
            Dictionary<string, int> genreToIdMap = new();
            Dictionary<string, int> styleToIdMap = new();

            StreamWriter genreCsvWriter = new StreamWriter($"{targetFolder}{albumGenrePath}_{genreFileNumber}.csv", false, Encoding.UTF8);
            genreCsvWriter.WriteLine(albumGenreHeaders);

            StreamWriter styleCsvWriter = new StreamWriter($"{targetFolder}{albumStylePath}_{styleFileNumber}.csv", false, Encoding.UTF8);
            styleCsvWriter.WriteLine(albumStyleHeaders);

            using var xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            try
            {
                HashSet<string> skipTags = new HashSet<string>(["videos", "artists"]);
                while (xmlReader.Read())
                {
                    Album album = new Album();
                    bool abort = false;
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "master")
                    {
                        Helpers.ParseIntAndSet(xmlReader.GetAttribute("id") ?? "", (val) => album.Id = val);

                        using var innerReader = xmlReader.ReadSubtree();
                        while (innerReader.Read())
                        {
                            if (innerReader.NodeType == XmlNodeType.Element)
                            {
                                if (skipTags.Contains(innerReader.Name))
                                {
                                    innerReader.Skip();
                                    continue;
                                }

                                switch (innerReader.Name)
                                {
                                    case "title":
                                        if (string.IsNullOrEmpty(album.Title))
                                        {
                                            album.Title = innerReader.ReadString().Replace("\"", "\"\"");
                                        }
                                        break;
                                    case "data_quality":
                                        abort = innerReader.ReadString() == "Needs Major Changes";
                                        break;
                                    case "genre":
                                        string genreName = innerReader.ReadString().Replace("\"", "\"\"");
                                        if (!string.IsNullOrEmpty(genreName))
                                        {
                                            if (!genreToIdMap.ContainsKey(genreName))
                                            {
                                                genreToIdMap[genreName] = genreToIdMap.Count + 1;
                                            }
                                            album.Genres.Add(genreToIdMap[genreName]);
                                        }

                                        break;
                                    case "style":
                                        string styleName = innerReader.ReadString().Replace("\"", "\"\"");
                                        if (!styleToIdMap.ContainsKey(styleName))
                                        {
                                            styleToIdMap[styleName] = styleToIdMap.Count + 1;
                                        }
                                        album.Styles.Add(styleToIdMap[styleName]);
                                        break;
                                }
                            }
                        }
                    }

                    if (!abort || album.Id != 0 || !string.IsNullOrEmpty(album.Title))
                    {
                        WriteAlbumRelationToCsv(genreCsvWriter, album.Id, album.Genres);
                        genreCount += album.Genres.Count;

                        if (genreCount >= batchSize)
                        {
                            genreCsvWriter.Dispose();

                            genreFileNumber++;
                            genreCsvWriter = new StreamWriter($"{targetFolder}{albumGenrePath}_{genreFileNumber}.csv", false, Encoding.UTF8);
                            genreCsvWriter.WriteLine(albumGenreHeaders);
                            genreCount = 0;
                        }

                        WriteAlbumRelationToCsv(styleCsvWriter, album.Id, album.Styles);
                        styleCount += album.Styles.Count;

                        if (styleCount >= batchSize)
                        {
                            styleCsvWriter.Dispose();

                            styleFileNumber++;
                            styleCsvWriter = new StreamWriter($"{targetFolder}{albumStylePath}_{styleFileNumber}.csv", false, Encoding.UTF8);
                            styleCsvWriter.WriteLine(albumStyleHeaders);
                            styleCount = 0;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"\nFATAL XML ERROR: {ex.Message}");
                Console.WriteLine($"Failed near Line: {ex.LineNumber}, Position: {ex.LinePosition}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUNEXPECTED ERROR: {ex.Message}");
            }
            finally
            {
                if (genreCsvWriter != null)
                {
                    genreCsvWriter.Flush();
                    genreCsvWriter.Dispose();
                    Console.WriteLine("GenreWriter safely closed.");
                }

                if (styleCsvWriter != null)
                {
                    styleCsvWriter.Flush();
                    styleCsvWriter.Dispose();
                    Console.WriteLine("StyleWriter safely closed.");
                }
            }

            WriteGenresToCsv(genreToIdMap, targetFolder + genrePath);
            WriteStylesToCsv(styleToIdMap, targetFolder + stylePath);
        }

        private static int WriteAlbumRelationToCsv(StreamWriter writer, int albumId, List<int> entityIds)
        {
            foreach (int entity in entityIds)
            {
                writer.WriteLine($"{albumId},{entity}");
            }
            return entityIds.Count;
        }

        private static void WriteGenresToCsv(Dictionary<string, int> genres, string path)
        {
            using (var genreWriter = new StreamWriter($"{path}.csv", false, Encoding.UTF8))
            {
                genreWriter.WriteLine("Id,Name");
                foreach (KeyValuePair<string, int> kvp in genres)
                {
                    string safeKey = kvp.Key.Replace("\"", "\"\"");
                    genreWriter.WriteLine($"{kvp.Value},\"{safeKey}\"");
                }
            }
        }
        private static void WriteStylesToCsv(Dictionary<string, int> styles, string path)
        {
            using (var styleWriter = new StreamWriter($"{path}.csv", false, Encoding.UTF8))
            {
                styleWriter.WriteLine("Id,Name");
                foreach (KeyValuePair<string, int> kvp in styles)
                {
                    string safeKey = kvp.Key.Replace("\"", "\"\"");
                    styleWriter.WriteLine($"{kvp.Value},\"{safeKey}\"");
                }
            }   
        }

        private static void CreatePaths(string[] paths, string targetFolder)
        {
            foreach (string path in paths)
            {
                int slashIndex = path.IndexOf('/');
                string directoryPath = targetFolder + path.Substring(0, slashIndex);
                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
        }
    }
}