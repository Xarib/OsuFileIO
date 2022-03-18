using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader;

public abstract class OsuFileReader<THitObject> : IOsuFileReader<THitObject> where THitObject : IHitObject
{
    private const string orderExceptionMessage = "The File was not read in the correct order. ReadMethods have to be ordered like: 'Genral -> Editor -> Metadata -> Difficulty -> Events -> TimingPoints -> Colours -> HitObjects' or with 'ReadFile'. For re reads reset the reader";
    protected readonly StreamReader sr;
    protected string line;
    private OsuFileReaderOverride overrides;
    private OsuFileReaderOptions options;

    private bool disposed;

    protected OsuFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
    {
        this.sr = new(path);
        this.sr.ThrowArgumentExceptionIfEmpty();

        this.overrides = overrides;
        this.options = options ?? new OsuFileReaderOptions();
    }

    protected OsuFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
    {
        this.sr = new(stream);
        this.sr.ThrowArgumentExceptionIfEmpty();

        this.overrides = overrides;
        this.options = options ?? new OsuFileReaderOptions();
    }

    protected OsuFileReader(StreamReader sr, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
    {
        this.sr = sr;
        this.overrides = overrides;
        this.options = options ?? new OsuFileReaderOptions();
    }

    public abstract IReadOnlyBeatmap<THitObject> ReadFile();

    public void ResetReader()
    {
        this.sr.Reset();
    }

    protected int ParseInt(string line)
    {
        if (this.options.IntParsing == IntParsing.Default)
        {
            return int.Parse(line);
        }

        return (int)double.Parse(line);
    }

    protected int ParseInt(ReadOnlySpan<char> line)
    {
        if (this.options.IntParsing == IntParsing.Default)
        {
            return int.Parse(line);
        }

        return (int)double.Parse(line);
    }

    private int? ParseIntNullable(string line)
        => line is null ? null : ParseInt(line);

    protected static double ParseDouble(string line)
        => double.Parse(line);

    private static double? ParseDoubleNullable(string line)
        => line is null ? null : ParseDouble(line);

    private string ReadTagValue()
    {
        return this.line?.Substring(this.line.IndexOf(':') + 1).Trim();
    }

    public General ReadGeneral()
    {
        var general = new General();

        this.line = sr.ReadLineStartingWithOrNull("osu file format");

        if (this.line is null)
            throw new OsuFileReaderException(orderExceptionMessage);

        var generalOverride = this.overrides?.General;

        general.OsuFileFormat = generalOverride?.OsuFileFormat ?? ParseIntNullable(this.line.Substring(this.line.LastIndexOf("v", StringComparison.OrdinalIgnoreCase) + 1));

        this.line = sr.ReadLineStartingWithOrNull("StackLeniency:");
        general.StackLeniency = generalOverride?.StackLeniency ?? ParseDoubleNullable(this.ReadTagValue());

        //TODO better
        if (this.line is null)
        {
            general.StackLeniency = 0.7;
            this.ResetReader();
        }

        if (this.overrides?.General?.Mode is null)
        {
            this.line = sr.ReadLineStartingWithOrNull("Mode:");
            general.Mode = Enum.Parse<GameMode>(line
                .TrimStart()
                .Remove(0, "Mode:".Length)
                .Trim());
        }
        else
        {
            general.Mode = this.overrides.General.Mode;
        }

        return general;
    }

    public Metadata ReadMetadata()
    {
        this.CheckReadOrder("[Metadata]");

        Metadata metadata;

        if (this.overrides?.MetaData is not null)
        {
            metadata = this.overrides.MetaData;
        }
        else
        {
            metadata = new Metadata();
        }

        while(this.ReadNextKeyValue(out var keyValue))
        {
            switch (keyValue.Key)
            {
                case "Title":
                    if (metadata.Title is not null)
                        break;

                    metadata.Title = keyValue.Value;
                    break;
                case "TitleUnicode":
                    if (metadata.TitleUnicode is not null)
                        break;

                    metadata.TitleUnicode = keyValue.Value;
                    break;
                case "Artist":
                    if (metadata.Artist is not null)
                        break;

                    metadata.Artist = keyValue.Value;
                    break;
                case "ArtistUnicode":
                    if (metadata.ArtistUnicode is not null)
                        break;

                    metadata.ArtistUnicode = keyValue.Value;
                    break;
                case "Creator":
                    if (metadata.Creator is not null)
                        break;

                    metadata.Creator = keyValue.Value;
                    break;
                case "Version":
                    if (metadata.Version is not null)
                        break;

                    metadata.Version = keyValue.Value;
                    break;
                case "Source":
                    if (metadata.Source is not null)
                        break;

                    metadata.Source = keyValue.Value;
                    break;
                case "Tags":
                    if (metadata.Tags is not null)
                        break;

                    metadata.Tags = keyValue.Value;
                    break;
                case "BeatmapID":
                    if (metadata.BeatmapID.HasValue)
                        break;

                    metadata.BeatmapID = ParseIntNullable(keyValue.Value);
                    break;
                case "BeatmapSetID":
                    if (metadata.BeatmapSetID.HasValue)
                        break;

                    metadata.BeatmapSetID = ParseIntNullable(keyValue.Value);
                    break;
                case "Genre":
                case "Language":
                    //New osu lazer values??
                    break;
                default:
                    throw new OsuFileReaderException($"Unkown key '{keyValue.Key}'");
            }
        }

        return metadata;
    }

    public Difficulty ReadDifficulty()
    {
        this.CheckReadOrder("[Difficulty]");

        Difficulty difficulty;

        if (this.overrides?.Difficulty is not null)
        {
            difficulty = this.overrides.Difficulty;
        }
        else
        {
            difficulty = new Difficulty();
        }

        while(this.ReadNextKeyValue(out var keyValue))
        {
            switch (keyValue.Key)
            {
                case "ApproachRate":
                    if (difficulty.ApproachRate.HasValue)
                        break;

                    difficulty.ApproachRate = ParseDoubleNullable(keyValue.Value);
                    break;
                case "CircleSize":
                    if (difficulty.CircleSize.HasValue)
                        break;

                    difficulty.CircleSize = ParseDoubleNullable(keyValue.Value);
                    break;
                case "HPDrainRate":
                    if (difficulty.HPDrainRate.HasValue)
                        break;

                    difficulty.HPDrainRate = ParseDoubleNullable(keyValue.Value);
                    break;
                case "OverallDifficulty":
                    if (difficulty.OverallDifficulty.HasValue)
                        break;

                    difficulty.OverallDifficulty = ParseDoubleNullable(keyValue.Value);
                    break;
                case "SliderMultiplier":
                    if (difficulty.SliderMultiplier.HasValue)
                        break;

                    difficulty.SliderMultiplier = ParseDoubleNullable(keyValue.Value);
                    break;
                case "SliderTickRate":
                    if (difficulty.SliderTickRate.HasValue)
                        break;

                    difficulty.SliderTickRate = ParseDoubleNullable(keyValue.Value);
                    break;
                default:
                    throw new OsuFileReaderException($"Unkown key '{keyValue.Key}'");
            }
        }

        if (!difficulty.ApproachRate.HasValue)
            difficulty.ApproachRate = 6;

        return difficulty;
    }

    private void CheckReadOrder(string dilimiter)
    {
        if (this.line is null || !this.line.StartsWith(dilimiter))
            this.line = this.sr.ReadLineStartingWithOrNull(dilimiter);

        if (this.line is null)
            throw new OsuFileReaderException(orderExceptionMessage);
    }

    private bool ReadNextKeyValue(out (string Key, string Value) keyValue)
    {
        this.line = this.sr.ReadLine();

        if (string.IsNullOrWhiteSpace(this.line) || this.line.StartsWith('['))
        {
            keyValue = (null, null);
            return false;
        }

        var indexColon = line.IndexOf(':');

        //Change this to while loop if more reads are needed
        if (indexColon == -1)
        {
            this.line = this.sr.ReadLine();
            indexColon = line.IndexOf(':');
        }

        keyValue = (this.line.Substring(0, indexColon), this.line.Substring(indexColon + 1));

        return true;
    }

    public List<TimingPoint> ReadTimingPoints()
    {
        var points = new List<TimingPoint>();

        if (this.line is null || !this.line.StartsWith("[TimingPoints]"))
            this.line = this.sr.ReadLineStartingWithOrNull("[TimingPoints]");

        if (this.line is null)
            throw new OsuFileReaderException(orderExceptionMessage);

        this.line = this.sr.ReadLine();

        double prevBeatLength = -1;
        while (!string.IsNullOrWhiteSpace(this.line) && !this.line.StartsWith('['))
        {
            var timingPoint = new TimingPoint();

            int spanIndex = -1;
            foreach (var span in this.line.SplitLinesAt(','))
            {
                spanIndex++;
                switch (spanIndex)
                {
                    case 0:
                        timingPoint.TimeInMs = ParseInt(span);
                        continue;
                    case 1:
                        var beatLength = double.Parse(span);

                        if (double.IsNaN(beatLength)) //Some apsire map
                            continue;

                        if (beatLength < 0)
                        { //Inherited Point (green)
                            if (prevBeatLength < 0) //Inherited timingpoint has to have a point to inherit
                            {
                                if (this.options.StrictTimingPointInheritance)
                                {
                                    throw new OsuFileReaderException($"{nameof(InheritedPoint)} has no {nameof(TimingPoint)} to inherit");
                                }
                                else
                                {
                                    timingPoint.BeatLength = prevBeatLength = 333.333333333333; // 180 Bpm
                                }
                            }

                            timingPoint = new InheritedPoint(timingPoint, beatLength)
                            {
                                BeatLength = prevBeatLength
                            };
                        }
                        else
                        { //Timinigpoint (red)
                            timingPoint.BeatLength = prevBeatLength = beatLength;
                        }

                        continue;
                    case 2:
                        timingPoint.Meter = ParseInt(span);
                        continue;
                    default:
                        break;
                }
                break;
            }

            points.Add(timingPoint);
            this.line = this.sr.ReadLine();
        }

        return points;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
            return;

        if (disposing)
        {
            this.sr.Dispose();
        }

        this.options = null;
        this.overrides = null;

        this.disposed = true;
    }
}

public class OsuFileReaderOverride
{
    public General General { get; set; }
    //TODO property Editor
    public Metadata MetaData { get; set; }
    public Difficulty Difficulty { get; set; }
    //TODO property Events
}

public class OsuFileReaderOptions
{
    public IntParsing IntParsing { get; set; } = IntParsing.ConvertFloat;
    public bool StrictTimingPointInheritance { get; set; } = false;
}
