using OsuFileIO.HitObject.Mania;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public class ManiaFileReader : OsuFileReader<ManiaHitObject>
    {
        public ManiaFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(path, options, overrides)
        {
        }

        public ManiaFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(stream, options, overrides)
        {
        }

        internal ManiaFileReader(StreamReader sr, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(sr, options, overrides)
        {
        }

        public override ReadOnlyBeatmap<ManiaHitObject> ReadFile()
        {
            var osuFile = new ReadOnlyBeatmap<ManiaHitObject>();

            try
            {
                osuFile.General = this.ReadGeneral();
                osuFile.MetaData = this.ReadMetadata();
                osuFile.Difficulty = this.ReadDifficulty();
                osuFile.TimingPoints = this.ReadTimingPoints();

                this.Dispose();

                return osuFile;
            }
            catch (Exception e)
            {
                throw new OsuFileReaderException($"The reader encountered an error at line: {this.line}, in file with beatmapId: {osuFile.MetaData.BeatmapID}, with title: {osuFile.MetaData.Title}", e);
            }
        }
    }
}
