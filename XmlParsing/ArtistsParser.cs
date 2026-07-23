using System.Xml;

namespace XmlParsing
{
    internal static class ArtistsParser
    {
        private static readonly HashSet<string> SkipTags = ["groups", "aliases", "namevariations", "urls"];

        internal static void ArtistsToCsv(string xmlFilePath, string csvFileName)
        {
            using var csv = new BatchedCsvWriter(csvFileName, "Id,Name,RealName,Profile");
            using var xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings
            {
                IgnoreWhitespace = true
            });

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "artist") continue;

                int id = 0;
                string name = "", realName = "", profile = "";
                bool lowQuality = false;

                using var innerReader = xmlReader.ReadSubtree();
                while (innerReader.Read())
                {
                    while (innerReader.NodeType == XmlNodeType.Element && SkipTags.Contains(innerReader.Name))
                    {
                        innerReader.Skip();
                    }

                    if (innerReader.NodeType != XmlNodeType.Element) continue;

                    if (innerReader.Depth != 1) continue;

                    switch (innerReader.Name)
                    {
                        case "id":
                            id = Helpers.ParseIntOrNull(innerReader.ReadString()) ?? 0;
                            break;
                        case "name":
                            name = innerReader.ReadString();
                            break;
                        case "realname":
                            realName = innerReader.ReadString();
                            break;
                        case "profile":
                            profile = innerReader.ReadString();
                            break;
                        case "data_quality":
                            lowQuality = innerReader.ReadString() == "Needs Major Changes";
                            break;
                    }
                }

                if (!lowQuality && id != 0 && !string.IsNullOrEmpty(name))
                {
                    csv.WriteLine($"{id},\"{name.EscapeCsv()}\",\"{realName.EscapeCsv()}\",\"{profile.EscapeCsv()}\"");
                }
            }
        }
    }
}
