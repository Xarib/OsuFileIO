using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public class OsuStdFileReader : OsuFileReader
    {
        public OsuStdFileReader(string path) : base(path)
        {
        }

        public OsuStdFileReader(Stream stream) : base(stream)
        {
        }

        public override OsuStdFile ReadAll()
        {
            var osuStdFile = new OsuStdFile();

            try
            {
                #region General
                var general = new General();

                this.line = sr.ReadLineStartingWithOrNull("osu file format");
                general.OsuFileFormat = ForceParse(this.line.Substring(this.line.LastIndexOf("v", StringComparison.OrdinalIgnoreCase) + 1));

                general.Mode = GameMode.Standard;

                this.line = sr.ReadLineStartingWithOrNull("StackLeniency:");
                general.StackLeniency = double.Parse(this.ReadTagValue());

                osuStdFile.General = general;
                #endregion

                #region MetaData
                var blockDict = this.ReadAllTagsInBlockOrNull("[Metadata]");

                osuStdFile.MetaData = new MetaData
                {
                    Artist = blockDict.GetValueOrDefault("Artist"),
                    ArtistUnicode = blockDict.GetValueOrDefault("ArtistUnicode"),
                    BeatmapID = ForceParse(blockDict.GetValueOrDefault("BeatmapID")),
                    BeatmapSetID = ForceParse(blockDict.GetValueOrDefault("BeatmapSetID")),
                    Creator = blockDict.GetValueOrDefault("Creator"),
                    Source = blockDict.GetValueOrDefault("Source"),
                    Tags = blockDict.GetValueOrDefault("Tags").Split(' '),
                    Title = blockDict.GetValueOrDefault("Title"),
                    TitleUnicode = blockDict.GetValueOrDefault("TitleUnicode"),
                    Version = blockDict.GetValueOrDefault("Version"),
                };
                #endregion

                #region Difficulty
                blockDict = this.ReadAllTagsInBlockOrNull("[Difficulty]");

                osuStdFile.Difficulty = new Difficulty
                {
                    ApproachRate = double.Parse(blockDict.GetValueOrDefault("ApproachRate")),
                    CircleSize = double.Parse(blockDict.GetValueOrDefault("CircleSize")),
                    HPDrainRate = double.Parse(blockDict.GetValueOrDefault("HPDrainRate")),
                    OverallDifficulty = double.Parse(blockDict.GetValueOrDefault("OverallDifficulty")),
                    SliderMultiplier = double.Parse(blockDict.GetValueOrDefault("SliderMultiplier")),
                    SliderTickRate = double.Parse(blockDict.GetValueOrDefault("SliderTickRate")),
                };
                #endregion

                #region TimingPoints
                this.line = this.sr.ReadLineStartingWithOrNull("[TimingPoints]");
                this.line = this.sr.ReadLine();
                while (this.line.Trim() != "" && !this.line.StartsWith('['))
                {
                    var timingPoint = new TimingPoint();

                    int spanIndex = -1;
                    foreach (var span in this.line.SplitLinesAt(','))
                    {
                        spanIndex++;
                        switch (spanIndex)
                        {
                            case 0:
                                timingPoint.TimeInMs = (int)double.Parse(span);
                                continue;
                            case 1:
                                timingPoint.BeatLength = double.Parse(span);
                                continue;
                            case 2:
                                timingPoint.Meter = (int)double.Parse(span);
                                continue;
                            default:
                                break;
                        }
                        break;
                    }

                    osuStdFile.TimingPoints.Add(timingPoint);
                    this.line = this.sr.ReadLine();
                }
                #endregion

                #region HitObjects

                this.line = this.sr.ReadLineStartingWithOrNull("[HitObjects]");
                int indexOfComma;
                while (!this.sr.EndOfStream)
                {
                    this.line = this.sr.ReadLine();

                    //Some maps have random new lines.
                    if (this.line.Trim() == "")
                        continue;

                    string hitobjectPart = line[0..line.IndexOf(',')];
                    indexOfComma = hitobjectPart.Length + 1;
                    int x = (int)double.Parse(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int y = (int)double.Parse(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int ms = (int)double.Parse(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int objectType = (int)double.Parse(hitobjectPart) % 4;

                    switch (objectType)
                    {
                        case 0:
                            osuStdFile.HitObjects.Add(ReadSpinner(new Coordinates(x, y), ms, line[indexOfComma..]));
                            break;
                        case 1:
                            osuStdFile.HitObjects.Add(new Circle(new Coordinates(x, y), ms));
                            break;
                        case 2:
                            osuStdFile.HitObjects.Add(ReadSlider(new Coordinates(x, y), ms, line[indexOfComma..]));
                            break;
                        default:
                            throw new OsuFileReaderException("Invalid type was found in string: " + objectType);
                    }
                }
                #endregion

            }
            catch (Exception e)
            {
                throw new OsuFileReaderException($"The reader encountert an Error at line: {this.line}, in File with beatmapId: {osuStdFile.MetaData.BeatmapID}, with Title: {osuStdFile.MetaData.Title}", e);
            }

            return osuStdFile;
        }

        private static Spinner ReadSpinner(Coordinates coordinates, int ms, string rest)
        {
            Spinner spinner = null;
            int spanIndex = -1;
            foreach (var span in rest.SplitLinesAt(','))
            {
                spanIndex++;

                switch (spanIndex)
                {
                    case 0:
                        continue;
                    case 1:
                        spinner = new Spinner(coordinates, ms, (int)double.Parse(span));
                        break;
                    default:
                        break;
                }
                break;
            }

            return spinner;
        }

        private static Slider ReadSlider(Coordinates coordinates, int ms, string rest)
        {
            Slider slider = null;
            int spanIndex = -1;
            var sliderPoints = new List<Coordinates>();
            foreach (var span in rest.SplitLinesAt(','))
            {
                spanIndex++;

                switch (spanIndex)
                {
                    case 0:
                        continue;
                    case 1:
                        var enumerator = span.ToString().SplitLinesAt('|');
                        enumerator.MoveNext();
                        foreach (var point in enumerator)
                        {
                            var x = (int)double.Parse(point[0..point.IndexOf(':')]);
                            var y = (int)double.Parse(point[(point.IndexOf(':') + 1)..]);

                            if (sliderPoints.Count == 0)
                            {
                                sliderPoints.Add(new Coordinates(x, y));
                                continue;
                            }

                            if (sliderPoints.Last() == new Coordinates(x, y))
                                continue;

                            sliderPoints.Add(new Coordinates(x, y));
                        }
                        continue;
                    case 2:
                        continue;
                    case 3:
                        slider = new Slider(coordinates, ms, sliderPoints, double.Parse(span));
                            break;
                    default:
                        break;
                }
                break;
            }

            return slider;
        }
    }
}
