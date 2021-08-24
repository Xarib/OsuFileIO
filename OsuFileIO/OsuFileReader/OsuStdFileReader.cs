using OsuFileIO.OsuFile;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public class OsuStdFileReader : OsuFileReader
    {
        public OsuStdFileReader(string path) : base(path)
        {
        }

        public OsuStdFileReader(Stream stream) : base(stream)
        {
        }

        public override OsuStdFile ReadAll()
        {
            var osuStdFile = new OsuStdFile();

            do
            {
                this.sr.ReadLine();
            } while (!this.sr.EndOfStream);

            return osuStdFile;
        }
    }
}
