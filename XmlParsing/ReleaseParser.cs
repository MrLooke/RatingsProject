using System.IO.Compression;
using System.Text;
using System.Xml;

namespace XmlParsing
{
    public static class ReleaseParser
    {
        internal class ReleaseFormat
        {
            public int Id { get; set; }

            public int MainId { get; set; }

            public string Format { get; set; } = "";

            public string Title { get; set; } = "";

            public List<string> Descriptions = new();
        }

        public static void Inspection(string xmlZipPath)
        {
            using FileStream fileStream = File.OpenRead(xmlZipPath);
            using GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using XmlReader reader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            });

            int releaseCount = 0;
            StringBuilder currentRelease = new StringBuilder();
            bool capturing = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "release")
                {
                    capturing = true;
                    currentRelease.Clear();
                }

                if (capturing)
                    currentRelease.Append(reader.ReadOuterXml());

                if (capturing)
                {
                    Console.WriteLine(currentRelease.ToString());
                    releaseCount++;
                    capturing = false;

                    if (releaseCount >= 3) break;
                }
            }
        }

        public static void InspectionForArtist(string xmlZipPath, string artistName, string outputPath)
        {
            using FileStream fileStream = File.OpenRead(xmlZipPath);
            using GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using XmlReader reader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            });

            var outputSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  "
            };

            using XmlWriter writer = XmlWriter.Create(outputPath, outputSettings);
            writer.WriteStartDocument();
            writer.WriteStartElement("releases");

            int matchCount = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "release")
                {
                    string releaseXml = reader.ReadOuterXml();

                    if (ReleaseContainsArtist(releaseXml, artistName))
                    {
                        // Re-read the captured fragment and write it into the output document
                        using XmlReader fragmentReader = XmlReader.Create(
                            new StringReader(releaseXml),
                            new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });

                        writer.WriteNode(fragmentReader, defattr: true);
                        matchCount++;
                    }
                }
            }

            writer.WriteEndElement(); // </releases>
            writer.WriteEndDocument();

            Console.WriteLine($"Exported {matchCount} releases for '{artistName}' to {outputPath}");
        }

        private static bool ReleaseContainsArtist(string releaseXml, string artistName)
        {
            using XmlReader releaseReader = XmlReader.Create(new StringReader(releaseXml));

            bool insideArtists = false;

            while (releaseReader.Read())
            {
                if (releaseReader.NodeType == XmlNodeType.Element && releaseReader.Name == "artists")
                    insideArtists = true;

                if (releaseReader.NodeType == XmlNodeType.EndElement && releaseReader.Name == "artists")
                    insideArtists = false;

                if (insideArtists
                    && releaseReader.NodeType == XmlNodeType.Element
                    && releaseReader.Name == "name")
                {
                    string name = releaseReader.ReadElementContentAsString();
                    if (string.Equals(name, artistName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public static void ReleasesToCsv(string xmlZipPath, string csvFileName)
        {
            int fileNumber = 1;
            int count = 0;
            int batchSize = 100000;
            HashSet<int> seen = new HashSet<int>(2000000);

            const string headers = "Id,MainId,Format";

            StreamWriter csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
            csvWriter.WriteLine(headers);

            using FileStream fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var xmlReader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            bool insideFormats = false;
            try
            {
                while (xmlReader.Read())
                {
                    ReleaseFormat format = new ReleaseFormat();

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "release")
                    {
                        Helpers.ParseIntAndSet(xmlReader.GetAttribute("id") ?? "", (val) => format.Id = val);

                        using var innerReader = xmlReader.ReadSubtree();

                        while (innerReader.Read())
                        {
                            if(innerReader.Name == "formats" && innerReader.NodeType == XmlNodeType.Element)
                            {
                                insideFormats = true;
                            }

                            if (innerReader.Name == "formats" && innerReader.NodeType == XmlNodeType.EndElement)
                            {
                                insideFormats = false;
                            }


                            if (innerReader.NodeType != XmlNodeType.Element)
                            {
                                innerReader.Skip();
                                continue;
                            }

                            if (innerReader.Name == "master_id")
                            {
                                Helpers.ParseIntAndSet(innerReader.ReadString(), (val) => format.MainId = val);
                            }

                            if (innerReader.Name == "title")
                            {
                                string title = innerReader.ReadString() ?? "";
                                if(!string.IsNullOrEmpty(title))
                                {
                                    format.Title = title;
                                }
                            }


                            if (insideFormats && innerReader.Name == "description")
                            {
                                string desc = innerReader.ReadString() ?? "";
                                if (!string.IsNullOrEmpty(desc))
                                {
                                    format.Descriptions.Add(desc);
                                }
                            }
                        }
                    }

                    if (format.Id != 0 && format.MainId != 0 && format.Descriptions.Count > 0)
                    {
                        if (seen.Contains(format.MainId)) continue;

                        format.Format = NormalizeFormat(format.Descriptions) ?? "";

                        if (string.IsNullOrEmpty(format.Format)) continue;
                        seen.Add(format.MainId);

                        csvWriter.WriteLine($"{format.Id},{format.MainId},\"{format.Format}\"");
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
                    Console.WriteLine("ReleaseWriter safely closed.");
                }
            }
        }


        private static string? NormalizeFormat(IEnumerable<string> descriptions) =>
            descriptions.Select(d => d.ToLower()).Select(d => d switch
            {
                "album" or "lp" or "mini-album" or "minimax" => "Album",
                "ep" or "maxi-single" => "EP",
                "single" => "Single",
                "compilation" => "Compilation",
                "mixtape" => "Mixtape",
                "sampler" => "Sampler",
                _ => null
            })
            .FirstOrDefault(f => f != null);
        }
}
