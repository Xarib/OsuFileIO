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
        private readonly IInterpretation result;
        private StdHitObjectReader reader;

        public OsuStdInterpreter(IInterpretation source)
        {
            this.result = source ?? new OsuStdInterpretation();
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
                        this.result.HitCircleCount++;
                        break;
                    case StdHitObjectType.Slider:
                        this.result.SliderCount++;
                        break;
                    case StdHitObjectType.Spinner:
                        this.result.SpinnerCount++;
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
                    this.result.Length = TimeSpan.FromMilliseconds(this.reader.CurrentHitObject.TimeInMs);
                    break;
                case StdHitObjectType.Slider:
                    var slider = this.reader.CurrentHitObject as Slider;
                    double sliderTime = slider.Length / this.reader.SliderVelocity * this.reader.CurrentTimingPoint.BeatLength;
                    this.result.Length = TimeSpan.FromMilliseconds(sliderTime + slider.TimeInMs);
                    break;
                case StdHitObjectType.Spinner:
                    var spinner = this.reader.CurrentHitObject as Spinner;
                    this.result.Length = TimeSpan.FromMilliseconds(spinner.EndTimeInMs);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unimplemented enum {this.reader.HitObjectType}");
            }

            totalTimeByBpm[lastRedTimingPoint.BeatLength] += Convert.ToInt32(this.result.Length.TotalMilliseconds - lastRedTimingPoint.TimeInMs);

            var longestTimeForBpms = 0;
            this.result.BpmMax = int.MinValue;
            this.result.BpmMin = int.MaxValue;
            foreach (var item in totalTimeByBpm)
            {
                var currentBpm = Convert.ToInt32(1 / item.Key * 60000d);
                if (item.Value > longestTimeForBpms)
                {
                    longestTimeForBpms = item.Value;
                    this.result.Bpm = currentBpm;
                }

                if (currentBpm > this.result.BpmMax)
                    this.result.BpmMax = currentBpm;

                if (currentBpm < this.result.BpmMin)
                    this.result.BpmMin = currentBpm;
            }
        }

        private const double beatLength100Bpm = 60000 / 100 / 4; //100 Bmp
        private const double beatLength150Bpm = 60000 / 150 / 4; //150 Bmp
        private void InterpretCountValues()
        {
            var previousHitObject = this.reader.GetHitObjectOrNull(offsetFromCurrent: -1);

            if (previousHitObject is null)
                return;

            var timeBetweenHitObjects = this.reader.CurrentHitObject.TimeInMs - previousHitObject.TimeInMs;


            if (timeBetweenHitObjects < this.reader.TimeBetweenOneTwoJumps && timeBetweenHitObjects > this.reader.TimeBetweenOneTwoJumps)
            {
                //InterpretOneTwoCount
            }

            if (timeBetweenHitObjects < this.reader.TimeBetweenStreamAlike * 1.1 && timeBetweenHitObjects <= beatLength100Bpm)
            {


                if (timeBetweenHitObjects <= beatLength150Bpm)
                    this.InterpretStreamCount();
            }
            else
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

            var nextHitObject = this.reader.GetHitObjectOrNull(offsetFromCurrent: 1);

            if (nextHitObject is not null && this.IsMappedLikeStream(nextHitObject.TimeInMs - this.reader.CurrentHitObject.TimeInMs))
                return;

            //Only count when stream is finished

            //If stream is longer than 8 beats. Has to be beat snap divisor: 1/4
            if (this.hitObjectCountStream > 32)
            {
                this.result.DeathStreamCount++;
            }
            //If stream is longer than 4 beats. Has to be beat snap divisor: 1/4
            else if (this.hitObjectCountStream > 16)
            {
                this.result.LongStreamCount++;
            }
            //If stream is longer than 2 beats. Has to be beat snap divisor: 1/4
            else if (this.hitObjectCountStream > 8)
            {
                this.result.StreamCount++;
            }
            else if (this.hitObjectCountStream > 4 && this.hitObjectCountStream < 9)
            {
                this.result.BurstCount++;
                return;
            }

            if (this.hitObjectCountStream > this.result.LongestStream)
                this.result.LongestStream = this.hitObjectCountStream;
        }
        private bool IsMappedLikeStream(double timeDifference)
        {
            return timeDifference < this.reader.TimeBetweenStreamAlike * 1.1 && timeDifference <= beatLength150Bpm;
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
            public int DoubleCount { get; set; }
            public int TripletCount { get; set; }
            public int QuadrupletCount { get; set; }
            public int BurstCount { get; set; }
            public int StreamCount { get; set; }
            public int LongStreamCount { get; set; }
            public int DeathStreamCount { get; set; }
            public int LongestStream { get; set; }
        }
    }
}
