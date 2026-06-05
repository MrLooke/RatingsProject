using System.Text;
using System.Xml;

namespace XmlParsing
{
    internal static class ArtistsParser
    {
        internal static void ArtistsToCsv(string xmlFilePath, string csvFileName)
        {
            int fileNumber = 1;
            int count = 0;
            int batchSize = 100000;
            const string headers = "Id,Name,RealName,Profile";

            StreamWriter csvWriter = new StreamWriter($"{csvFileName}_{fileNumber}.csv", false, Encoding.UTF8);
            csvWriter.WriteLine(headers);

            using var xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            try
            {
                HashSet<string> skipTags = new HashSet<string>(["groups", "aliases", "namevariations", "urls"]);
                while (xmlReader.Read())
                {
                    Artist artist = new Artist();
                    bool abort = false;
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "artist")
                    {
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

                                if(innerReader.Depth == 1)
                                {
                                    string currentTag = innerReader.Name;
                                    switch (currentTag)
                                    {
                                        case "id":
                                            Helpers.ParseIntAndSet(innerReader.ReadString(), (val) => artist.Id = val);
                                            break;
                                        case "name":
                                            artist.Name = innerReader.ReadString();
                                            break;
                                        case "realname":
                                            artist.RealName = innerReader.ReadString();
                                            break;
                                        case "profile":
                                            artist.Profile = innerReader.ReadString();
                                            break;
                                        case "data_quality":
                                            abort = innerReader.ReadString() == "Needs Major Changes";
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (!abort && artist.Id != 0 && !string.IsNullOrEmpty(artist.Name))
                    {
                        string safeName = artist.Name?.Replace("\"", "\"\"") ?? "";
                        string safeRealName = artist.RealName?.Replace("\"", "\"\"") ?? "";
                        string safeProfile = artist.Profile?.Replace("\"", "\"\"") ?? "";

                        csvWriter.WriteLine($"{artist.Id},\"{safeName}\",\"{safeRealName}\",\"{safeProfile}\"");
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
            catch(XmlException ex)
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
                    Console.WriteLine("ArtistWriter safely closed.");
                }
            }
        }
    }

    internal class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? RealName { get; set; } = "";
        public string? Profile { get; set; }
    }
}
