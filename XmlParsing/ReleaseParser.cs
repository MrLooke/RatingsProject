using System.IO.Compression;
using System.Text;
using System.Xml;
using static XmlReaders.MastersParsers;

namespace XmlParsing
{
    public static class ReleaseParser
    {
        internal class ReleaseFormat
        {
            public int Id { get; set; }

            public int MainId { get; set; }

            public string Format { get; set; } = "";

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

        public static void ReleasesToCsv(string xmlZipPath, string csvFileName)
        {
            int fileNumber = 1;
            int count = 0;
            int batchSize = 100000;
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
                while (xmlReader.Read() && fileNumber < 10)
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


                            if (innerReader.NodeType != XmlNodeType.Element
                                || (innerReader.Name == "master_id" && innerReader.GetAttribute("is_main_release") != "true"))
                            {
                                innerReader.Skip();
                                continue;
                            }

                            if (innerReader.Name == "master_id")
                            {
                                Helpers.ParseIntAndSet(innerReader.ReadString(), (val) => format.MainId = val);
                            }
                            
                           
                            if(insideFormats && innerReader.Name == "description")
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
                        format.Format = NormalizeFormat(format.Descriptions) ?? "";

                        if (string.IsNullOrEmpty(format.Format)) continue;

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
                    Console.WriteLine("AlbumWriter safely closed.");
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
