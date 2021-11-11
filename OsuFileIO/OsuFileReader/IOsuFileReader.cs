using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader;

public interface IOsuFileReader<out THitObject> : IDisposable where THitObject : IHitObject
{
    public void ResetReader();
    public General ReadGeneral();
    public Metadata ReadMetadata();
    public Difficulty ReadDifficulty();
    public List<TimingPoint> ReadTimingPoints();
    public abstract IReadOnlyBeatmap<THitObject> ReadFile();
}
