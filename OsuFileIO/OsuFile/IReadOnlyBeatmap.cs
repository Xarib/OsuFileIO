using OsuFileIO.HitObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFile
{
    public interface IReadOnlyBeatmap<out THitObject> where THitObject : IHitObject
    {
        public General General { get; set; }
        //TODO property Editor
        public MetaData MetaData { get; set; }
        public Difficulty Difficulty { get; set; }
        //TODO property Events
        public List<TimingPoint> TimingPoints { get; set; }
        //TODO property Combo colors
        public IReadOnlyList<THitObject> HitObjects { get; }
    }
}
