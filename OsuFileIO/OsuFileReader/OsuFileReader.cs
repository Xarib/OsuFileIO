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
    }
}
