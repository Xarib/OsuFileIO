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

                if (lastRedTimingPoint is null && this.reader.CurrentTimingPoint is InheritedPoint)
                {
                    lastRedTimingPoint = beatmap.TimingPoints[0];
                    totalTimeByBpm.Add(lastRedTimingPoint.BeatLength, 0);
                }
                else if (lastRedTimingPoint is null && this.reader.CurrentTimingPoint is not InheritedPoint)
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

                this.InterpretCountValues();

            } while (this.reader.ReadNext());

            switch (this.reader.HitObjectType)
            {
                case StdHitObjectType.Circle:

                    this.result.Length = TimeSpan.FromMilliseconds(this.reader.CurrentHitObject.TimeInMs);

                    break;
                case StdHitObjectType.Slider:

                    var slider = this.reader.CurrentHitObject as Slider;
                    this.result.Length = TimeSpan.FromMilliseconds(this.CalculateSliderEndTime(slider, this.reader.CurrentTimingPoint));

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

        private double CalculateSliderEndTime(Slider slider, TimingPoint timingPoint)
            => slider.Length / this.reader.SliderVelocity * timingPoint.BeatLength + slider.TimeInMs;

        private const double beatLength100Bpm = 60000 / 100 / 4; //100 Bmp
        private const double beatLength150Bpm = 60000 / 150 / 4; //150 Bmp
        private void InterpretCountValues()
        {
            var previousHitObject = this.reader.GetHitObjectFromOffsetOrNull(-1);

            if (previousHitObject is null)
                return;

            var timeBetweenHitObjects = this.reader.CurrentHitObject.TimeInMs - previousHitObject.TimeInMs;

            if (timeBetweenHitObjects < this.reader.TimeBetweenOneTwoJumps && timeBetweenHitObjects > this.reader.TimeBetweenOneTwoJumps)
            {
                //InterpretOneTwoCount
            }

            if (this.IsMappedLikeDoubleToQuad(timeBetweenHitObjects))
            {
                this.InterpretDoubleToQuadCount();
            }
            else
            {
                //If it ends it goes here
                this.hitObjectCountDoubleToQuad = 1;
                this.IsCircleOnly = true;
            }

            if (this.IsMappedLikeStream(timeBetweenHitObjects))
            {
                this.InterpretStreamCount();
            }
        }

        private int hitObjectCountDoubleToQuad = 1;
        private bool IsCircleOnly = true;
        private void InterpretDoubleToQuadCount()
        {
            this.hitObjectCountDoubleToQuad++;

            var nextHitObject = this.reader.GetHitObjectFromOffsetOrNull(1);

            if (this.reader.HitObjectType != StdHitObjectType.Circle)
                this.IsCircleOnly = false;

            if (nextHitObject is not null && this.IsMappedLikeDoubleToQuad(nextHitObject.TimeInMs - this.reader.CurrentHitObject.TimeInMs))
                return;

            switch (this.hitObjectCountDoubleToQuad)
            {
                case 1:
                    break;
                case 2:
                    this.result.DoubleCount++;

                    if (this.IsCircleOnly && !this.IsDirectlyAfterSlider(offsetBeggining: -1))
                        this.result.TrueDoubleCount++;

                    break;
                case 3:
                    this.result.TripletCount++;

                    if (this.IsCircleOnly && !this.IsDirectlyAfterSlider(offsetBeggining: -2))
                        this.result.TrueTripletCount++;

                    break;
                case 4:
                    this.result.QuadrupletCount++;

                    if (this.IsCircleOnly && !this.IsDirectlyAfterSlider(offsetBeggining: -3))
                        this.result.TrueQuadrupletCount++;

                    break;
                default:
                    //TODO: Make use of this
                    break;
            }
        }

        private bool IsDirectlyAfterSlider(int offsetBeggining)
        {
            var history = this.reader.GetHistoryEntryOrNull(offsetBeggining - 1);

            if (history?.Item2 is not Slider slider)
                return false;

            var startingHitObject = this.reader.GetHitObjectFromOffsetOrNull(offsetBeggining);
            var timeDifference = startingHitObject.TimeInMs - this.CalculateSliderEndTime(slider, history?.Item1);

            return timeDifference < this.reader.TimeBetweenStreamAlike * 1.1;
        }

        private bool IsMappedLikeDoubleToQuad(double timeBetweenHitObjects)
        {
            return timeBetweenHitObjects < this.reader.TimeBetweenStreamAlike * 1.1 && timeBetweenHitObjects <= beatLength100Bpm;
        }

        private int hitObjectCountStream = 1;
        private double streamPixels;
        private double spacedStreamPixels;
        private void InterpretStreamCount()
        {
            var distanceBetweenCurrentAndLastObject = GetDistanceBetweenTwoHitObjects(this.reader.CurrentHitObject, this.reader.GetHitObjectFromOffsetOrNull(-1));
            this.streamPixels += distanceBetweenCurrentAndLastObject;

            var circleDiameter = 2 * (54.4 - 4.48 * this.reader.CircleSize); //Formula => https://osu.ppy.sh/wiki/en/Beatmapping/Circle_size
            var spacePixels = distanceBetweenCurrentAndLastObject - circleDiameter;

            if (spacePixels > 0)
                this.spacedStreamPixels += spacePixels;

            this.hitObjectCountStream++;

            var hitObjectNext1 = this.reader.GetHitObjectFromOffsetOrNull(1);

            if (hitObjectNext1 is not null && this.IsMappedLikeStream(hitObjectNext1.TimeInMs - this.reader.CurrentHitObject.TimeInMs))
            {
                #region StreamJumps

                var hitObjectNext2 = this.reader.GetHitObjectFromOffsetOrNull(2);//TODO better name

                if (hitObjectNext2 is not null && this.IsMappedLikeStream(hitObjectNext2.TimeInMs - hitObjectNext1.TimeInMs))
                {
                    var distanceBetweenCurrentAndNext1 = GetDistanceBetweenTwoHitObjects(this.reader.CurrentHitObject, hitObjectNext1);
                    var distanceBetweenNext1AndNext2 = GetDistanceBetweenTwoHitObjects(hitObjectNext1, hitObjectNext2);

                    var ratioDistanceBeforeJump = distanceBetweenCurrentAndNext1 / distanceBetweenCurrentAndLastObject;
                    var ratioDistanceAfterJump = distanceBetweenCurrentAndNext1 / distanceBetweenNext1AndNext2;

                    if (distanceBetweenCurrentAndNext1 > circleDiameter &&
                        distanceBetweenCurrentAndNext1 > distanceBetweenCurrentAndLastObject &&
                        distanceBetweenCurrentAndNext1 > distanceBetweenNext1AndNext2 &&
                        ratioDistanceBeforeJump >= 1.2 &&
                        ratioDistanceAfterJump >= 1.2
                        )
                    {
                        this.result.StreamJumpCount++;
                    }
                }

                #endregion

                return;
            }

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
            }

            if (this.hitObjectCountStream > 4)
            {
                this.result.TotalStreamAlikePixels += this.streamPixels;

                this.result.TotalSpacedStreamAlikePixels += this.spacedStreamPixels;
            }

            if (this.hitObjectCountStream > 8 && this.hitObjectCountStream > this.result.LongestStream)
                this.result.LongestStream = this.hitObjectCountStream;

            //Reset
            this.hitObjectCountStream = 1;
            this.streamPixels = 0;
            this.spacedStreamPixels = 0;
        }
        private bool IsMappedLikeStream(double timeDifference)
        {
            return timeDifference < this.reader.TimeBetweenStreamAlike * 1.1 && timeDifference <= beatLength150Bpm;
        }

        private static double GetDistanceBetweenTwoHitObjects(IHitObject hitObject1, IHitObject hitObject2)
        {
            return Math.Sqrt((hitObject1.Coordinates.X - hitObject2.Coordinates.X) * (hitObject1.Coordinates.X - hitObject2.Coordinates.X) + (hitObject1.Coordinates.Y - hitObject2.Coordinates.Y) * (hitObject1.Coordinates.Y - hitObject2.Coordinates.Y));
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
            public int TrueDoubleCount { get; set; }
            public int TripletCount { get; set; }
            public int TrueTripletCount { get; set; }
            public int QuadrupletCount { get; set; }
            public int TrueQuadrupletCount { get; set; }
            public int BurstCount { get; set; }
            public int StreamCount { get; set; }
            public int LongStreamCount { get; set; }
            public int DeathStreamCount { get; set; }
            public int LongestStream { get; set; }
            public double TotalStreamAlikePixels { get; set; }
            public double TotalSpacedStreamAlikePixels { get; set; }
            public int StreamJumpCount { get; set; }
        }
    }
}
