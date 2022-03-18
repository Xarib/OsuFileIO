using OsuFileIO.Enums;
using OsuFileIO.HitObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFile;

public class ReadOnlyBeatmap<THitObject> : IReadOnlyBeatmap<THitObject> where THitObject : IHitObject
{
    public General General { get; internal set; }
    //TODO property Editor
    public Metadata MetaData { get; internal set; }
    public Difficulty Difficulty { get; internal set; }
    //TODO property Events
    public List<TimingPoint> TimingPoints { get; internal set; } = new();
    //TODO property Combo colors
    public IReadOnlyList<THitObject> HitObjects { get; internal set; }
}

public class General : IEquatable<General>
{
    //TODO missing props do not forget -> EpilepsyWarning
    public int? OsuFileFormat { get; set; }
    public GameMode? Mode { get; set; }
    public double? StackLeniency { get; set; }

    public bool Equals(General other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
            return false;

        return
            this.OsuFileFormat == other.OsuFileFormat &&
            this.Mode == other.Mode &&
            this.StackLeniency == other.StackLeniency;
    }

    public override bool Equals(object obj)
        => Equals(obj as General);

    public override int GetHashCode()
        => (this.OsuFileFormat, this.Mode, this.StackLeniency).GetHashCode();

    public static bool operator ==(General lhs, General rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(General lhs, General rhs) => !(lhs == rhs);
}

public class Metadata : IEquatable<Metadata>
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

    public bool Equals(Metadata other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
            return false;

        return
            this.Title == other.Title &&
            this.TitleUnicode == other.TitleUnicode &&
            this.Artist == other.Artist &&
            this.ArtistUnicode == other.ArtistUnicode &&
            this.Creator == other.Creator &&
            this.Version == other.Version &&
            this.Source == other.Source &&
            this.Tags == other.Tags &&
            this.BeatmapID == other.BeatmapID &&
            this.BeatmapSetID == other.BeatmapSetID;
    }

    public override bool Equals(object obj)
        => Equals(obj as Metadata);

    public override int GetHashCode()
        => (this.Title, this.TitleUnicode, this.Artist, this.ArtistUnicode, this.Creator, this.Version, this.Source, this.Tags, this.BeatmapID, this.BeatmapSetID).GetHashCode();

    public static bool operator ==(Metadata lhs, Metadata rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(Metadata lhs, Metadata rhs) => !(lhs == rhs);
}

public class Difficulty : IEquatable<Difficulty>
{
    public double? HPDrainRate { get; set; }
    public double? CircleSize { get; set; }
    public double? OverallDifficulty { get; set; }
    public double? ApproachRate { get; set; }
    public double? SliderMultiplier { get; set; }
    public double? SliderTickRate { get; set; }

    public bool Equals(Difficulty other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
            return false;

        return
            this.HPDrainRate == other.HPDrainRate &&
            this.CircleSize == other.CircleSize &&
            this.OverallDifficulty == other.OverallDifficulty &&
            this.ApproachRate == other.ApproachRate &&
            this.SliderMultiplier == other.SliderMultiplier &&
            this.SliderTickRate == other.SliderTickRate;
    }

    public override bool Equals(object obj)
        => Equals(obj as Difficulty);

    public override int GetHashCode()
        => (this.HPDrainRate, this.CircleSize, this.OverallDifficulty, this.ApproachRate, this.SliderMultiplier, this.SliderTickRate).GetHashCode();

    public static bool operator ==(Difficulty lhs, Difficulty rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(Difficulty lhs, Difficulty rhs) => !(lhs == rhs);
}

public class TimingPoint : IEquatable<TimingPoint>
{
    public int TimeInMs { get; set; }
    public double BeatLength { get; set; }
    public int Meter { get; set; }

    public bool Equals(TimingPoint other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
            return false;

        return
            this.TimeInMs == other.TimeInMs &&
            this.BeatLength == other.BeatLength &&
            this.Meter == other.Meter;
    }

    public override bool Equals(object obj)
        => Equals(obj as TimingPoint);

    public override int GetHashCode()
        => (this.TimeInMs, this.BeatLength, this.Meter).GetHashCode();

    public static bool operator ==(TimingPoint lhs, TimingPoint rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(TimingPoint lhs, TimingPoint rhs) => !(lhs == rhs);
}

public class InheritedPoint : TimingPoint, IEquatable<InheritedPoint>
{
    public double VelocityMultiplier { get; set; }

    public InheritedPoint()
    {

    }

    public InheritedPoint(TimingPoint timingPoint, double multiplier)
    {
        this.BeatLength = timingPoint.BeatLength;
        this.Meter = timingPoint.Meter;
        this.TimeInMs = timingPoint.TimeInMs;
        this.VelocityMultiplier = -100d / multiplier;
    }

    public bool Equals(InheritedPoint other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
            return false;

        return
            this.TimeInMs == other.TimeInMs &&
            this.BeatLength == other.BeatLength &&
            this.Meter == other.Meter &&
            this.VelocityMultiplier == other.VelocityMultiplier;
    }

    public override bool Equals(object obj)
        => Equals(obj as InheritedPoint);

    public override int GetHashCode()
        => (this.TimeInMs, this.BeatLength, this.Meter, this.VelocityMultiplier).GetHashCode();

    public static bool operator ==(InheritedPoint lhs, InheritedPoint rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
                return true;

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(InheritedPoint lhs, InheritedPoint rhs) => !(lhs == rhs);
}
