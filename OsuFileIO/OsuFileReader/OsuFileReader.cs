using OsuFileIO.Extensions;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public abstract class OsuFileReader
    {
        protected readonly StreamReader sr;
        protected string line;

        public OsuFileReader(string path)
        {
            this.sr = new(path);
        }

        public OsuFileReader(Stream stream)
        {
            this.sr = new(stream);
        }

        public void Dispose()
        {
            this.sr.Dispose();
        }

        public abstract OsuFile.OsuFile ReadAll();

        /// <summary>
        /// this parses a flaot string to an int. The flaot is truncated
        /// </summary>
        /// <returns></returns>
        protected static int? ForceParse(string line)
        {
            if (line is null)
                return null;

            return (int)float.Parse(line);
        }

        protected string ReadTagValue()
        {
            return this.line.Substring(this.line.IndexOf(':') + 1).Trim();
        }

        protected Dictionary<string, string> ReadAllTagsInBlockOrNull(string dilimiter)
        {
            if (!this.line.StartsWith(dilimiter))
                this.line = this.sr.ReadLineStartingWithOrNull(dilimiter);

            var tagDict = new Dictionary<string, string>();

            int indexColon;
            this.line = this.sr.ReadLine();
            while (this.line.Trim() != "" && !this.line.StartsWith('['))
            {
                indexColon = line.IndexOf(':');

                tagDict.Add(this.line.Substring(0, indexColon), this.line.Substring(indexColon + 1));

                this.line = this.sr.ReadLine();
            }

            return tagDict;
        }
    }
}
