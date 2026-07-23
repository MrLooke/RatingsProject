using System.IO.Compression;
using System.Xml;

namespace XmlParsing
{
    internal static class ReleaseParser
    {
        internal static void ReleasesToCsv(string xmlZipPath, string csvFileName)
        {
            using var csv = new BatchedCsvWriter(csvFileName, "Id,MainId,Format");
            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var xmlReader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            var seenMasters = new HashSet<int>(2_000_000);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "release") continue;

                int id = Helpers.ParseIntOrNull(xmlReader.GetAttribute("id")) ?? 0;
                int mainId = 0;
                var descriptions = new List<string>();
                bool insideFormats = false;

                using var innerReader = xmlReader.ReadSubtree();
                while (innerReader.Read())
                {
                    if (innerReader.Name == "formats")
                    {
                        if (innerReader.NodeType == XmlNodeType.Element) insideFormats = true;
                        else if (innerReader.NodeType == XmlNodeType.EndElement) insideFormats = false;
                    }

                    if (innerReader.NodeType != XmlNodeType.Element) continue;

                    switch (innerReader.Name)
                    {
                        case "master_id":
                            mainId = Helpers.ParseIntOrNull(innerReader.ReadString()) ?? 0;
                            break;
                        case "description" when insideFormats:
                            string description = innerReader.ReadString();
                            if (!string.IsNullOrEmpty(description))
                            {
                                descriptions.Add(description);
                            }
                            break;
                    }
                }

                if (id == 0 || mainId == 0 || seenMasters.Contains(mainId)) continue;

                string? format = NormalizeFormat(descriptions);
                if (format is null) continue;

                seenMasters.Add(mainId);
                csv.WriteLine($"{id},{mainId},\"{format}\"");
            }
        }

        internal static void PrintSampleReleases(string xmlZipPath, int count = 3)
        {
            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            });

            int printed = 0;
            while (printed < count && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "release")
                {
                    Console.WriteLine(reader.ReadOuterXml());
                    printed++;
                }
            }
        }

        internal static void ExportReleasesForArtist(string xmlZipPath, string artistName, string outputPath)
        {
            using var fileStream = File.OpenRead(xmlZipPath);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = XmlReader.Create(gzipStream, new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            });

            using var writer = XmlWriter.Create(outputPath, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  "
            });
            writer.WriteStartDocument();
            writer.WriteStartElement("releases");

            int matchCount = 0;

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element || reader.Name != "release") continue;

                string releaseXml = reader.ReadOuterXml();
                if (!ReleaseContainsArtist(releaseXml, artistName)) continue;

                using var fragmentReader = XmlReader.Create(
                    new StringReader(releaseXml),
                    new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });

                writer.WriteNode(fragmentReader, defattr: true);
                matchCount++;
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            Console.WriteLine($"Exported {matchCount} releases for '{artistName}' to {outputPath}");
        }

        private static bool ReleaseContainsArtist(string releaseXml, string artistName)
        {
            using var releaseReader = XmlReader.Create(new StringReader(releaseXml));

            bool insideArtists = false;

            while (releaseReader.Read())
            {
                if (releaseReader.Name == "artists")
                {
                    if (releaseReader.NodeType == XmlNodeType.Element) insideArtists = true;
                    else if (releaseReader.NodeType == XmlNodeType.EndElement) insideArtists = false;
                }

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

        private static string? NormalizeFormat(IEnumerable<string> descriptions) =>
            descriptions.Select(d => d.ToLowerInvariant() switch
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
