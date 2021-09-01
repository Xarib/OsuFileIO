using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public abstract class OsuFileReader
    {
        private const string orderExceptionMessage = "The File was not read in the correct order. ReadMethods have to be ordere like: 'Genral -> Editor -> Metadata -> Difficulty -> Events -> TimingPoints -> Colours -> HitObjects' or with 'ReadFile'. No repetions";
        protected readonly StreamReader sr;
        protected string line;
        private readonly OsuFileReaderOverride overrides;
        private readonly OsuFileReaderOptions options;

        public OsuFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        {
            this.sr = new(path);
            this.overrides = overrides;
            this.options = options ?? new OsuFileReaderOptions
            {
                IntParsing = IntParsing.ConvertFloat,
                StringComparison = StringComparison.OrdinalIgnoreCase,
            };
        }

        public OsuFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
        {
            this.sr = new(stream);
            this.overrides = overrides;
            this.options = options ?? new OsuFileReaderOptions
            {
                IntParsing = IntParsing.ConvertFloat,
                StringComparison = StringComparison.OrdinalIgnoreCase,
            };
        }

        public abstract OsuFile.OsuFile ReadFile();

        public void Dispose()
            => this.sr.Dispose();

        public void ResetReader()
        {
            this.sr.BaseStream.Position = 0;
            this.sr.DiscardBufferedData();
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

        protected double ParseDouble(string line)
            => double.Parse(line);

        private double? ParseDoubleNullable(string line)
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
            general.StackLeniency = this.ParseDoubleNullable(this.ReadTagValue());

            if (this.overrides?.General?.Mode == null)
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

            return new MetaData
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
        }

        public Difficulty ReadDifficulty()
        {
            var blockDict = this.ReadAllTagsInBlockOrNull("[Difficulty]");

            return new Difficulty
            {
                ApproachRate = this.ParseDoubleNullable(blockDict.GetValueOrDefault("ApproachRate")),
                CircleSize = this.ParseDoubleNullable(blockDict.GetValueOrDefault("CircleSize")),
                HPDrainRate = this.ParseDoubleNullable(blockDict.GetValueOrDefault("HPDrainRate")),
                OverallDifficulty = this.ParseDoubleNullable(blockDict.GetValueOrDefault("OverallDifficulty")),
                SliderMultiplier = this.ParseDoubleNullable(blockDict.GetValueOrDefault("SliderMultiplier")),
                SliderTickRate = this.ParseDoubleNullable(blockDict.GetValueOrDefault("SliderTickRate")),
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
                            timingPoint.BeatLength = double.Parse(span);
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
    }
}
