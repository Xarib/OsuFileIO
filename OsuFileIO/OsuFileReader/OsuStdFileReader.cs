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
                int indexOfComma;
                while (this.line.Trim() != "" && !this.line.StartsWith('['))
                {
                    var timingPoint = new TimingPoint();

                    string s0 = line[0..line.IndexOf(',')];
                    indexOfComma = s0.Length + 1;
                    timingPoint.TimeInMs = (int)double.Parse(s0);

                    s0 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += s0.Length + 1;
                    timingPoint.BeatLength = double.Parse(s0);

                    s0 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += s0.Length + 1;
                    timingPoint.Meter = (int)double.Parse(s0);

                    osuStdFile.TimingPoints.Add(timingPoint);

                    this.line = this.sr.ReadLine();
                }
                #endregion

                #region HitObjects
                //string s0 = line[0..line.IndexOf(',')];
                //indexOfComma = s0.Length + 1;
                //string s1 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                //indexOfComma += s1.Length + 1;
                //string s2 = line[indexOfComma..line.IndexOf(',', indexOfComma)];

                this.line = this.sr.ReadLineStartingWithOrNull("[HitObjects]");
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
                    int timeInMs = (int)double.Parse(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int objectType = (int)double.Parse(hitobjectPart) % 4;

                    IHitObject hitObject;
                    switch (objectType)
                    {
                        case 0:
                            hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                            indexOfComma += hitobjectPart.Length + 1;
                            hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];

                            hitObject = new Spinner(new Coordinates(x, y), timeInMs, (int)double.Parse(hitobjectPart));
                            break;

                        case 1:
                            hitObject = new Circle(new Coordinates(x, y), timeInMs);
                            break;

                        case 2:
                            hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                            indexOfComma += hitobjectPart.Length + 1;

                            hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                            indexOfComma += hitobjectPart.Length + 1;

                            var sliderParts = hitobjectPart.Split('|');
                            var sliderPoints = new List<Coordinates>();
                            string[] coordsAsString;
                            for (int i = 1; i < sliderParts.Length; i++)
                            {
                                coordsAsString = sliderParts[i].Split(':');
                                var point = new Coordinates
                                {
                                    X = (int)double.Parse(coordsAsString[0]),
                                    Y = (int)double.Parse(coordsAsString[1]),
                                };

                                var lastPoint = sliderPoints.LastOrDefault();
                                if (i == 1)
                                    lastPoint = new Coordinates(-1, -1);

                                if (point == lastPoint)
                                    continue;

                                sliderPoints.Add(point);
                            }

                            hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                            indexOfComma += hitobjectPart.Length + 1;

                            if (this.line == "499,284,39769,2,0,L|425:306,1,53.9999983520508")
                                ;

                            var tempIndex = line.IndexOf(',', indexOfComma);

                            if (tempIndex == -1)
                            {
                                hitobjectPart = line[indexOfComma..];
                            }
                            else
                            {
                                hitobjectPart = line[indexOfComma..tempIndex];
                            }

                            hitObject = new Slider(new Coordinates(x, y), timeInMs, sliderPoints, double.Parse(hitobjectPart));
                            break;

                        default:
                            throw new OsuFileReaderException("Invalid type was found in string: " + objectType);
                    }

                    osuStdFile.HitObjects.Add(hitObject);
                }
                #endregion

            }
            catch (Exception e)
            {
                throw new OsuFileReaderException($"The reader encountert an Error at line: {this.line}, in File with beatmapId: {osuStdFile.MetaData.BeatmapID}, with Title: {osuStdFile.MetaData.Title}", e);
            }

            return osuStdFile;
        }
    }
}
