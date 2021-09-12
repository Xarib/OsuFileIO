using OsuFileIO.HitObject;
using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public class OsuStdInterpreter
    {
        private readonly IInterpretation source;
        private StdHitObjectReader reader;

        public OsuStdInterpreter(IInterpretation source)
        {
            this.source = source ?? new OsuStdInterpretation();
        }

        public void Interpret(OsuStdFile beatmap)
        {
            this.reader = new StdHitObjectReader(beatmap.Difficulty, beatmap.TimingPoints, beatmap.HitObjects);

            Dictionary<double, int> totalTimeByBpm = new();
            TimingPoint lastRedTimingPoint = null;
            do
            {
                switch (this.reader.HitObjectType)
                {
                    case StdHitObjectType.Circle:
                        this.source.HitCircleCount++;
                        break;
                    case StdHitObjectType.Slider:
                        this.source.SliderCount++;
                        break;
                    case StdHitObjectType.Spinner:
                        this.source.SpinnerCount++;
                        break;
                    default:
                        throw new InvalidEnumArgumentException($"Unimplemented enum {this.reader.HitObjectType}");
                }

                if (this.reader.CurrentTimingPoint is not InheritedPoint)
                {
                    if (lastRedTimingPoint is null)
                    {
                        totalTimeByBpm.Add(this.reader.CurrentTimingPoint.BeatLength, 0);
                        lastRedTimingPoint = this.reader.CurrentTimingPoint;
                    }
                    else
                    {
                        var timeDurationLastPoint = this.reader.CurrentTimingPoint.TimeInMs - lastRedTimingPoint.TimeInMs;
                        totalTimeByBpm[lastRedTimingPoint.BeatLength] += timeDurationLastPoint;

                        if (!totalTimeByBpm.ContainsKey(this.reader.CurrentTimingPoint.BeatLength))
                            totalTimeByBpm.Add(this.reader.CurrentTimingPoint.BeatLength, 0);

                        lastRedTimingPoint = this.reader.CurrentTimingPoint;
                    }
                }

                this.InterpretCountValues();

            } while (this.reader.ReadNext());

            switch (this.reader.HitObjectType)
            {
                case StdHitObjectType.Circle:
                    this.source.Length = TimeSpan.FromMilliseconds(this.reader.CurrentHitObject.TimeInMs);
                    break;
                case StdHitObjectType.Slider:
                    var slider = this.reader.CurrentHitObject as Slider;
                    double sliderTime = slider.Length / this.reader.SliderVelocity * this.reader.CurrentTimingPoint.BeatLength;
                    this.source.Length = TimeSpan.FromMilliseconds(sliderTime + slider.TimeInMs);
                    break;
                case StdHitObjectType.Spinner:
                    var spinner = this.reader.CurrentHitObject as Spinner;
                    this.source.Length = TimeSpan.FromMilliseconds(spinner.EndTimeInMs);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unimplemented enum {this.reader.HitObjectType}");
            }

            totalTimeByBpm[lastRedTimingPoint.BeatLength] += Convert.ToInt32(this.source.Length.TotalMilliseconds - lastRedTimingPoint.TimeInMs);

            var longestTimeForBpms = 0;
            this.source.BpmMax = int.MinValue;
            this.source.BpmMin = int.MaxValue;
            foreach (var item in totalTimeByBpm)
            {
                var currentBpm = Convert.ToInt32(1 / item.Key * 60000d);
                if (item.Value > longestTimeForBpms)
                {
                    longestTimeForBpms = item.Value;
                    this.source.Bpm = currentBpm;
                }

                if (currentBpm > this.source.BpmMax)
                    this.source.BpmMax = currentBpm;

                if (currentBpm < this.source.BpmMin)
                    this.source.BpmMin = currentBpm;
            }
        }

        
        private const double minimumDifferenceStreamObject = 60000 / 150 / 4; //150 Bmp
        private void InterpretCountValues()
        {
            var previousHitObject = this.reader.GetHitObject(offsetFromCurrent: -1);

            if (previousHitObject is null)
                return;

            var timeBetweenHitObjects = this.reader.CurrentHitObject.TimeInMs - previousHitObject.TimeInMs;


            if (timeBetweenHitObjects < this.reader.TimeBetweenOneTwoJumps && timeBetweenHitObjects > this.reader.TimeBetweenOneTwoJumps)
            {
                //InterpretOneTwoCount
            }
            else if (timeBetweenHitObjects < this.reader.TimeBetweenStreamObjects * 1.1 && timeBetweenHitObjects <= minimumDifferenceStreamObject)
            {
                this.InterpretStreamCount();
            } else
            {
                //If stream ends it goes here
                this.hitObjectCountStream = 1;
            }
        }

        private int hitObjectCountStream = 1;
        private void InterpretStreamCount()
        {
            if (this.hitObjectCountStream > 1)
            {
                //TODO fancy angle calculations
            }

            this.hitObjectCountStream++;

            switch (this.hitObjectCountStream)
            {
                case 1:
                    return;
                case 2:
                    return;
                case 3:
                    return;
                case 4:
                    return;
                //double etc
                default:
                    //Stream counting
                    if (this.hitObjectCountStream < 9)
                    {
                        //Butstcount
                        return;
                    }
                    break;
            }

            if (this.hitObjectCountStream > this.source.LongestStream)
                this.source.LongestStream = this.hitObjectCountStream;
        }

        private class OsuStdInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
            public int HitCircleCount { get; set; }
            public int SliderCount { get; set; }
            public int SpinnerCount { get; set; }
            public double Bpm { get; set; }
            public double BpmMin { get; set; }
            public double BpmMax { get; set; }
            public int LongestStream { get; set; }
        }
    }
}
