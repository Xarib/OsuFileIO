using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.Exceptions;
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
        public OsuStdFileReader(string path, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null)
            : base(path, options, overrides)
        {
        }

        public OsuStdFileReader(Stream stream, OsuFileReaderOptions options = null, OsuFileReaderOverride overrides = null) 
            : base(stream, options, overrides)
        {
        }

        public override OsuStdFile ReadFile()
        {
            var osuStdFile = new OsuStdFile();

            try
            {
                osuStdFile.General = this.ReadGeneral();
                osuStdFile.MetaData = this.ReadMetadata();
                osuStdFile.Difficulty = this.ReadDifficulty();
                osuStdFile.TimingPoints = this.ReadTimingPoints();

                if (this.line is null || !this.line.StartsWith("[HitObjects]"))
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
                    int x = this.ParseInt(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int y = this.ParseInt(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int ms = this.ParseInt(hitobjectPart);

                    hitobjectPart = line[indexOfComma..line.IndexOf(',', indexOfComma)];
                    indexOfComma += hitobjectPart.Length + 1;
                    int objectType = this.ParseInt(hitobjectPart) % 4;

                    switch (objectType)
                    {
                        case 0:
                            osuStdFile.HitObjects.Add(this.ReadSpinner(new Coordinates(x, y), ms, line[indexOfComma..]));
                            break;
                        case 1:
                            osuStdFile.HitObjects.Add(new Circle(new Coordinates(x, y), ms));
                            break;
                        case 2:
                            osuStdFile.HitObjects.Add(this.ReadSlider(new Coordinates(x, y), ms, line[indexOfComma..]));
                            break;
                        default:
                            throw new OsuFileReaderException("Invalid type was found in string: " + objectType);
                    }
                }
            }
            catch (Exception e)
            {
                this.Dispose();
                throw new OsuFileReaderException($"The reader encountert an Error at line: {this.line}, in File with beatmapId: {osuStdFile.MetaData.BeatmapID}, with Title: {osuStdFile.MetaData.Title}", e);
            }

            this.Dispose();

            return osuStdFile;
        }

        private Spinner ReadSpinner(Coordinates coordinates, int ms, string rest)
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
                        spinner = new Spinner(coordinates, ms, this.ParseInt(span));
                        break;
                    default:
                        break;
                }
                break;
            }

            return spinner;
        }

        private Slider ReadSlider(Coordinates coordinates, int ms, string rest)
        {
            Slider slider = null;
            int spanIndex = -1;
            var sliderPoints = new List<Coordinates>();
            char curveType = '_';
            int slides = -1;

            foreach (var span in rest.SplitLinesAt(','))
            {
                spanIndex++;

                switch (spanIndex)
                {
                    case 0:
                        continue;
                    case 1:
                        var enumerator = span.ToString().SplitLinesAt('|');

                        curveType = span[0];

                        enumerator.MoveNext();

                        foreach (var point in enumerator)
                        {
                            var x = this.ParseInt(point[0..point.IndexOf(':')]);
                            var y = this.ParseInt(point[(point.IndexOf(':') + 1)..]);

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
                        slides = int.Parse(span);
                        continue;
                    case 3:
                        slider = new Slider(coordinates, ms, sliderPoints, double.Parse(span), (CurveType)curveType, slides);
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
