using OsuFileIO.Enums;
using OsuFileIO.Extensions;
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
                    timingPoint.TimeInMs = (int)float.Parse(s0);

                    s0 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += s0.Length + 1;
                    timingPoint.BeatLength = float.Parse(s0);

                    s0 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += s0.Length + 1;
                    timingPoint.Meter = (int)float.Parse(s0);

                    osuStdFile.TimingPoints.Add(timingPoint);
                }
                #endregion

                #region HitObjects
                //string s0 = line[0..line.IndexOf(',')];
                //indexOfComma = s0.Length + 1;
                //string s1 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                //indexOfComma += s1.Length + 1;
                //string s2 = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                #endregion

                do
                {
                    this.line = sr.ReadLine();
                } while (!this.sr.EndOfStream);

            }
            catch (Exception e)
            {
                throw new OsuFileReaderException($"The reader encountert an Error at line: {this.line}, in File with beatmapId: {osuStdFile.MetaData.BeatmapID}, with Title: {osuStdFile.MetaData.Title}", e);
            }

            return osuStdFile;
        }
    }
}
