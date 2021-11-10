using OsuFileIO.HitObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFile;

public interface IReadOnlyBeatmap<out THitObject> where THitObject : IHitObject
{
    public General General { get; }
    //TODO property Editor
    public MetaData MetaData { get; }
    public Difficulty Difficulty { get; }
    //TODO property Events
    public List<TimingPoint> TimingPoints { get; }
    //TODO property Combo colors
    public IReadOnlyList<THitObject> HitObjects { get; }
}
