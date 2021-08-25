using OsuFileIO.Enums;
using OsuFileIO.HitObject;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFile
{
    public abstract class OsuFile
    {
        public General General { get; set; }
        //TODO property Editor
        public MetaData MetaData { get; set; }
        public Difficulty Difficulty { get; set; }
        //TODO property Events
        public List<TimingPoint> TimingPoints { get; set; } = new();
        //TODO property Combo colors
        public List<IHitObject> HitObjects { get; set; } = new();
    }

    public class General
    {

        //TODO missing props do not forget -> EpilepsyWarning
        public int? OsuFileFormat { get; set; }
        public GameMode Mode { get; set; }
        public double? StackLeniency { get; set; }
    }

    public class MetaData
    {
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }
        public string[] Tags { get; set; }
        public int? BeatmapID { get; set; }
        public int? BeatmapSetID { get; set; }
    }

    public class Difficulty
    {
        public double? HPDrainRate { get; set; }
        public double? CircleSize { get; set; }
        public double? OverallDifficulty { get; set; }
        public double? ApproachRate  { get; set; }
        public double? SliderMultiplier { get; set; }
        public double? SliderTickRate { get; set; }
    }
}
