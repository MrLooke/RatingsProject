namespace XmlParsing
{
    internal static class Helpers
    {
        internal static int? ParseIntOrNull(string? value) =>
            int.TryParse(value, out var result) ? result : null;

        internal static string EscapeCsv(this string? value) =>
            (value ?? "").Replace("\"", "\"\"");
    }
}
