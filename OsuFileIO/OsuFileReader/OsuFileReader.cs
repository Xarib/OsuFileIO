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

namespace OsuFileIO.OsuFileReader
{
    public abstract class OsuFileReader<THitObject> : IOsuFileReader<THitObject> where THitObject : IHitObject
    {
        private const string orderExceptionMessage = "The File was not read in the correct order. ReadMethods have to be ordere like: 'Genral -> Editor -> Metadata -> Difficulty -> Events -> TimingPoints -> Colours -> HitObjects' or with 'ReadFile'. For repetitions reset the reader";
        protected readonly StreamReader sr;
        protected string line;
        private OsuFileReaderOverride overrides;
        private OsuFileReaderOptions options;

        private bool disposed;

        private static readonly OsuFileReaderOptions defaultOptions = new OsuFileReaderOptions
        {
            IntParsing = IntParsing.ConvertFloat,
            StringComparison = StringComparison.OrdinalIgnoreCase,
            StrictTimingPointInheritance = false,
        };

        protected OsuFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        {
            this.sr = new(path);
            this.sr.ThrowArgumentExceptionIfEmpty();

            this.overrides = overrides;
            this.options = options ?? defaultOptions;
        }

        protected OsuFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        {
            this.sr = new(stream);
            this.sr.ThrowArgumentExceptionIfEmpty();

            this.overrides = overrides;
            this.options = options ?? defaultOptions;
        }

        protected OsuFileReader(StreamReader sr, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        {
            this.sr = sr;
            this.overrides = overrides;
            this.options = options ?? defaultOptions;
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

        private Dictionary<string, string> ReadAllTagsInBlockOrNull(string dilimiter)
        {
            if (this.line is null || !this.line.StartsWith(dilimiter))
                this.line = this.sr.ReadLineStartingWithOrNull(dilimiter);

            if (this.line is null)
                throw new OsuFileReaderException(orderExceptionMessage);

            var tagDict = new Dictionary<string, string>();

            int indexColon;
            this.line = this.sr.ReadLine();
            while (!string.IsNullOrWhiteSpace(this.line) && !this.line.StartsWith('['))
            {
                indexColon = line.IndexOf(':');

                if (indexColon < 0)
                {
                    this.line = this.sr.ReadLine();
                    continue;
                }

                tagDict.Add(this.line.Substring(0, indexColon), this.line.Substring(indexColon + 1));

                this.line = this.sr.ReadLine();
            }

            return tagDict;
        }

        public General ReadGeneral()
        {
            var general = new General();

            this.line = sr.ReadLineStartingWithOrNull("osu file format");

            if (this.line is null)
                throw new OsuFileReaderException(orderExceptionMessage);

            general.OsuFileFormat = ParseIntNullable(this.line.Substring(this.line.LastIndexOf("v", StringComparison.OrdinalIgnoreCase) + 1));

            this.line = sr.ReadLineStartingWithOrNull("StackLeniency:");
            general.StackLeniency = ParseDoubleNullable(this.ReadTagValue());

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

        public MetaData ReadMetadata()
        {
            var blockDict = this.ReadAllTagsInBlockOrNull("[Metadata]");

            var metadata = new MetaData
            {
                Artist = blockDict.GetValueOrDefault("Artist"),
                ArtistUnicode = blockDict.GetValueOrDefault("ArtistUnicode"),
                BeatmapID = ParseIntNullable(blockDict.GetValueOrDefault("BeatmapID")),
                BeatmapSetID = ParseIntNullable(blockDict.GetValueOrDefault("BeatmapSetID")),
                Creator = blockDict.GetValueOrDefault("Creator"),
                Source = blockDict.GetValueOrDefault("Source"),
                Tags = blockDict.GetValueOrDefault("Tags"),
                Title = blockDict.GetValueOrDefault("Title"),
                TitleUnicode = blockDict.GetValueOrDefault("TitleUnicode"),
                Version = blockDict.GetValueOrDefault("Version"),
            };

            if (this.overrides?.MetaData?.BeatmapID is not null)
                metadata.BeatmapID = this.overrides.MetaData.BeatmapID.Value;

            return metadata;
        }

        public Difficulty ReadDifficulty()
        {
            var blockDict = this.ReadAllTagsInBlockOrNull("[Difficulty]");

            return new Difficulty
            {
                ApproachRate = ParseDoubleNullable(blockDict.GetValueOrDefault("ApproachRate")),
                CircleSize = ParseDoubleNullable(blockDict.GetValueOrDefault("CircleSize")),
                HPDrainRate = ParseDoubleNullable(blockDict.GetValueOrDefault("HPDrainRate")),
                OverallDifficulty = ParseDoubleNullable(blockDict.GetValueOrDefault("OverallDifficulty")),
                SliderMultiplier = ParseDoubleNullable(blockDict.GetValueOrDefault("SliderMultiplier")),
                SliderTickRate = ParseDoubleNullable(blockDict.GetValueOrDefault("SliderTickRate")),
            };
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
        public MetaData MetaData { get; set; }
        public Difficulty Difficulty { get; set; }
        //TODO property Events
    }

    public class OsuFileReaderOptions
    {
        public StringComparison StringComparison { get; set; }
        public IntParsing IntParsing { get; set; }
        public bool StrictTimingPointInheritance { get; set; }
    }
}
