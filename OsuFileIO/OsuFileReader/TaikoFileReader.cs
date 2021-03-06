using OsuFileIO.HitObject.Taiko;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader;

public class TaikoFileReader : OsuFileReader<TaikoHitObject>
{
    public TaikoFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        : base(path, options, overrides)
    {
    }

    public TaikoFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        : base(stream, options, overrides)
    {
    }

    internal TaikoFileReader(StreamReader sr, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        : base(sr, options, overrides)
    {
    }

    public override ReadOnlyBeatmap<TaikoHitObject> ReadFile()
    {
        var osuFile = new ReadOnlyBeatmap<TaikoHitObject>();

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
