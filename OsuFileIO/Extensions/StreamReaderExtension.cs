using OsuFileIO.OsuFileReader.Exceptions;
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


        private const int oByte = 111;
        private const int spaceByte = 32;
        /// <summary>
        /// Reset the Stream reader and skips BOM bytes if it exits.
        /// </summary>
        /// <param name="sr"></param>
        internal static void Reset(this StreamReader sr)
        {
            sr.BaseStream.Position = 0;

            int b = sr.BaseStream.ReadByte();

            if (b == oByte || b == spaceByte)
            {
                sr.BaseStream.Position = 0;
            }
            else if (b == Encoding.UTF8.Preamble[0])
            {
                sr.BaseStream.Position = 3;
            }
            else
            {
                while (b != oByte && b != spaceByte)
                {
                    if (sr.BaseStream.Position > 10)
                        throw new OsuFileReaderException("Failed to reset reader!");

                    b = sr.BaseStream.ReadByte();
                }

                sr.BaseStream.Seek(-1, SeekOrigin.Current);
            }

            sr.DiscardBufferedData();
        }

        internal static void ThrowArgumentExceptionIfEmpty(this StreamReader sr)
        {
            if (sr.BaseStream.Length == 0)
                throw new ArgumentException("The given file is empty");
        }
    }
}
