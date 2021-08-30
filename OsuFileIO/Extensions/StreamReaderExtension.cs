using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Extensions
{
    internal static class StreamReaderExtension
    {
        internal static string ReadLineStartingWithOrNull(this StreamReader sr, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            string line;
            do
            {
                line = sr
                    .ReadLine()
                    .Trim();
            }
            while (!sr.EndOfStream && !line.StartsWith(value, stringComparison));

            if (sr.EndOfStream)
                return null;

            return line;
        }
    }
}
