using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Extensions
{
    internal static class StringExtensions
    {
        public static SplitEnumerator SplitLinesAt(this string str, char delimiter)
        {
            return new SplitEnumerator(str.AsSpan(), delimiter);
        }
    }

    internal ref struct SplitEnumerator
    {
        private ReadOnlySpan<char> str;
        private readonly char delimiter;

        public ReadOnlySpan<char> Current { get; private set; }

        internal SplitEnumerator(ReadOnlySpan<char> str, char delimiter)
        {
            this.str = str;
            this.delimiter = delimiter;
            this.Current = default;
        }

        public bool MoveNext()
        {
            var span = this.str;

            if (span.Length == 0)
                return false;

            var index = span.IndexOf(delimiter);

            //If no dilimiters are left or exist
            if (index == -1)
            {
                this.str = ReadOnlySpan<char>.Empty;
                this.Current = span;
                return true;
            }

            this.Current = span.Slice(0, index); // string eqivelant [0..index] or Substring(0, index)
            this.str = span.Slice(index + 1); // means the span except the part we sliced before
            return true;
        }

        public SplitEnumerator GetEnumerator() => this;
    }
}
