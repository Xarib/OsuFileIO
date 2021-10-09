using OsuFileIO.HitObject.Catch;
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
    public class OsuCatchFileReader : OsuFileReader<CatchHitObject>
    {
        public OsuCatchFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(path, options, overrides)
        {
        }

        public OsuCatchFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(stream, options, overrides)
        {
        }

        public override ReadOnlyBeatmap<CatchHitObject> ReadFile()
        {
            var osuFile = new ReadOnlyBeatmap<CatchHitObject>();

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
                throw new OsuFileReaderException($"The reader encountert an Error at line: {this.line}, in File with beatmapId: {osuFile.MetaData.BeatmapID}, with Title: {osuFile.MetaData.Title}", e);
            }
        }
    }
}
