using System;
using System.Collections.Generic;
using System.Text;

namespace XmlParsing
{
    internal static class Helpers
    {
        internal static void ParseIntAndSet(string toParse, Action<int> setProp)
        {
            if (int.TryParse(toParse, out var result))
            {
                setProp(result);
            }
            else
            {
                Console.WriteLine("Invalid string to parse");
            }
        }
    }
}
