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

                this.InterpretJumps();

                this.InterpretSliders();

                this.InterpretMiscellaneous();

            } while (this.reader.ReadNext());

            //Stuff at end of map

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

            this.result.AvgSliderPointCount = (double)this.result.SliderPointCount / this.result.SliderCount;
        }

        private double CalculateSliderEndTime(Slider slider, TimingPoint timingPoint)
            => slider.Length / this.reader.SliderVelocity * timingPoint.BeatLength + slider.TimeInMs;
        private double CalculateSliderDuration(Slider slider, TimingPoint timingPoint)
            => slider.Length / this.reader.SliderVelocity * timingPoint.BeatLength;

        private const double beatLength100Bpm = 60000 / 100 / 4; //100 Bmp
        private const double beatLength150Bpm = 60000 / 150 / 4; //150 Bmp
        private void InterpretCountValues()
        {
            var previousHitObject = this.reader.GetHitObjectFromOffsetOrNull(-1);

            if (previousHitObject is null)
                return;

            var timeBetweenHitObjects = this.reader.CurrentHitObject.TimeInMs - previousHitObject.TimeInMs;

            if (timeBetweenHitObjects < this.reader.TimeHalfBeat && timeBetweenHitObjects > this.reader.TimeHalfBeat)
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
                        this.result.StandaloneDoubleCount++;

                    break;
                case 3:
                    this.result.TripletCount++;

                    if (this.IsCircleOnly && !this.IsDirectlyAfterSlider(offsetBeggining: -2))
                        this.result.StandaloneTripletCount++;

                    break;
                case 4:
                    this.result.QuadrupletCount++;

                    if (this.IsCircleOnly && !this.IsDirectlyAfterSlider(offsetBeggining: -3))
                        this.result.StandaloneQuadrupletCount++;

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

            return timeDifference < this.reader.TimeQuarterBeat * 1.1;
        }

        private bool IsMappedLikeDoubleToQuad(double timeBetweenHitObjects)
        {
            return timeBetweenHitObjects < this.reader.TimeQuarterBeat * 1.1;
        }

        private int hitObjectCountStream = 1;
        private double streamPixels = 0;
        private double spacedStreamPixels = 0;
        private int slidersInStream = 0;
        private void InterpretStreamCount()
        {
            var distanceBetweenCurrentAndLastObject = CalculateDistanceBetweenTwoHitObjects(this.reader.CurrentHitObject.Coordinates, this.reader.GetHitObjectFromOffsetOrNull(-1).Coordinates);
            this.streamPixels += distanceBetweenCurrentAndLastObject;

            var circleDiameter = 2 * (54.4 - 4.48 * this.reader.CircleSize); //Formula => https://osu.ppy.sh/wiki/en/Beatmapping/Circle_size
            var spacePixels = distanceBetweenCurrentAndLastObject - circleDiameter;

            if (spacePixels > 0)
                this.spacedStreamPixels += spacePixels;

            this.hitObjectCountStream++;

            if (this.reader.HitObjectType == StdHitObjectType.Slider)
                this.slidersInStream++;

            var hitObjectNext1 = this.reader.GetHitObjectFromOffsetOrNull(1);

            if (hitObjectNext1 is not null && this.IsMappedLikeStream(hitObjectNext1.TimeInMs - this.reader.CurrentHitObject.TimeInMs))
            {
                #region StreamJumps

                var hitObjectNext2 = this.reader.GetHitObjectFromOffsetOrNull(2);

                if (hitObjectNext2 is not null && this.IsMappedLikeStream(hitObjectNext2.TimeInMs - hitObjectNext1.TimeInMs))
                {
                    var distanceBetweenCurrentAndNext1 = CalculateDistanceBetweenTwoHitObjects(this.reader.CurrentHitObject.Coordinates, hitObjectNext1.Coordinates);
                    var distanceBetweenNext1AndNext2 = CalculateDistanceBetweenTwoHitObjects(hitObjectNext1.Coordinates, hitObjectNext2.Coordinates);

                    var ratioDistanceBeforeJump = distanceBetweenCurrentAndNext1 / distanceBetweenCurrentAndLastObject;
                    var ratioDistanceAfterJump = distanceBetweenCurrentAndNext1 / distanceBetweenNext1AndNext2;

                    if (distanceBetweenCurrentAndNext1 > circleDiameter &&
                        distanceBetweenCurrentAndNext1 > distanceBetweenCurrentAndLastObject &&
                        distanceBetweenCurrentAndNext1 > distanceBetweenNext1AndNext2 &&
                        ratioDistanceBeforeJump >= 1.2 &&
                        ratioDistanceAfterJump >= 1.2
                        )
                    {
                        this.result.StreamCutsCount++;
                    }
                }

                #endregion

                return;
            }

            //Only count when stream is finished

            //If stream is longer than 8 beats.
            if (this.hitObjectCountStream > 32)
            {
                this.result.DeathStreamCount++;
            }
            //If stream is longer than 4 beats.
            else if (this.hitObjectCountStream > 16)
            {
                this.result.LongStreamCount++;
            }
            //If stream is longer than 2 beats.
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

                this.result.SlidersInStreamAlike += this.slidersInStream;
            }

            if (this.hitObjectCountStream > 8 && this.hitObjectCountStream > this.result.LongestStream)
                this.result.LongestStream = this.hitObjectCountStream;

            //Reset
            this.hitObjectCountStream = 1;
            this.streamPixels = 0;
            this.spacedStreamPixels = 0;
            this.slidersInStream = 0;
        }
        private bool IsMappedLikeStream(double timeDifference)
        {
            return timeDifference < this.reader.TimeQuarterBeat * 1.1 && timeDifference <= beatLength100Bpm;
        }

        private void InterpretJumps()
        {
            var hitObjectPrev1 = this.reader.GetHitObjectFromOffsetOrNull(-1);
            var hitObjectPrev2 = this.reader.GetHitObjectFromOffsetOrNull(-2);

            if (hitObjectPrev1 is null)
                return;

            var distanceBetweenCurrentAndPrev = CalculateDistanceBetweenTwoHitObjects(this.reader.CurrentHitObject.Coordinates, hitObjectPrev1.Coordinates);

            if (distanceBetweenCurrentAndPrev >= 100)
                this.result.TotalJumpPixels += distanceBetweenCurrentAndPrev;

            if (distanceBetweenCurrentAndPrev >= 325)
                this.result.CrossScreenJumpCount++;

            if (hitObjectPrev2 is null ||
                distanceBetweenCurrentAndPrev < 100 ||
                CalculateDistanceBetweenTwoHitObjects(hitObjectPrev1.Coordinates, hitObjectPrev2.Coordinates) < 100 ||
                !this.IsMappedLikeJump(this.reader.CurrentHitObject.TimeInMs - hitObjectPrev1.TimeInMs) ||
                !this.IsMappedLikeJump(hitObjectPrev1.TimeInMs - hitObjectPrev2.TimeInMs))
                return;

            var degrees = CalcualteAngle(this.reader.CurrentHitObject.Coordinates, hitObjectPrev1.Coordinates, hitObjectPrev2.Coordinates);

            if (degrees <= 5 || degrees >= 175)
            {
                this.result.Jump180DegreesCount++;
            }
            else if (degrees >= 85 && degrees <= 95)
            {
                this.result.Jump90DegreesCount++;
            }
        }

        private void InterpretSliders()
        {
            if (this.reader.HitObjectType != StdHitObjectType.Slider)
                return;

            var slider = this.reader.CurrentHitObject as Slider;

            this.result.TotalSliderLength += slider.Length;

            this.result.SliderPointCount += slider.SliderCoordinates.Count;

            var sliderDuration = this.CalculateSliderDuration(slider, this.reader.CurrentTimingPoint);

            if (sliderDuration > this.reader.TimeEighthOfBeat * 1.5 && sliderDuration < this.reader.TimeQuarterBeat * 1.1)
                this.result.KickSliderCount++;

            switch (slider.CurveType)
            {
                case CurveType.Bézier:
                    this.result.BèzierSliderCount++;
                    break;
                case CurveType.CentripetalCatmullRom:
                    this.result.CatmullSliderCount++;
                    break;
                case CurveType.Linear:
                    this.result.LinearSliderCount++;
                    break;
                case CurveType.PerfectCircle:
                    this.result.PerfectCicleSliderCount++;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unimplemented case: " + slider.CurveType);
            }
        }

        private bool IsMappedLikeJump(double timeDifference)
        {
            return timeDifference < this.reader.TimeHalfBeat * 1.1;
        }

        private void InterpretMiscellaneous()
        {
            var hitObjectPrev1 = this.reader.GetHitObjectFromOffsetOrNull(-1);

            if (hitObjectPrev1 is null || hitObjectPrev1 is Spinner)
                return;

            if (this.reader.HitObjectType == StdHitObjectType.Circle && hitObjectPrev1 is Circle circle)
            {
                if (circle.Coordinates == this.reader.CurrentHitObject.Coordinates)
                    this.result.CirclePerfectStackCount++;
            }
            else if (hitObjectPrev1 is Slider prevSlider && this.reader.CurrentHitObject is Slider currentSlider)
            {
                if ((currentSlider.Coordinates == prevSlider.Coordinates && currentSlider.SliderCoordinates.Last() == prevSlider.SliderCoordinates.Last()) || 
                    (currentSlider.Coordinates == prevSlider.SliderCoordinates.Last() && currentSlider.SliderCoordinates.Last() == prevSlider.Coordinates))
                    this.result.SliderPerfectStackCount++;
            }
        }

        private static double CalculateDistanceBetweenTwoHitObjects(Coordinates coordinates1, Coordinates coordinates2)
        {
            return Math.Sqrt((coordinates1.X - coordinates2.X) * (coordinates1.X - coordinates2.X) + (coordinates1.Y - coordinates2.Y) * (coordinates1.Y - coordinates2.Y));
        }

        private static double CalcualteAngle(Coordinates current, Coordinates prev1, Coordinates prev2)
        {
            var degrees = Math.Atan2(prev2.Y - prev1.Y, prev2.X - prev1.X) -
                Math.Atan2(current.Y - prev1.Y, current.X - prev1.X);

            degrees *= 180d / Math.PI;

            if (degrees < 0)
                degrees *= -1d;

            if (degrees > 180)
                degrees = 360d - degrees;

            return degrees;
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
            public int StandaloneDoubleCount { get; set; }
            public int TripletCount { get; set; }
            public int StandaloneTripletCount { get; set; }
            public int QuadrupletCount { get; set; }
            public int StandaloneQuadrupletCount { get; set; }
            public int BurstCount { get; set; }
            public int StreamCount { get; set; }
            public int LongStreamCount { get; set; }
            public int DeathStreamCount { get; set; }
            public int LongestStream { get; set; }
            public double TotalStreamAlikePixels { get; set; }
            public double TotalSpacedStreamAlikePixels { get; set; }
            public int StreamCutsCount { get; set; }
            public int SlidersInStreamAlike { get; set; }
            public int Jump90DegreesCount { get; set; }
            public int Jump180DegreesCount { get; set; }
            public double TotalJumpPixels { get; set; }
            public int CrossScreenJumpCount { get; set; }
            public double TotalSliderLength { get; set; }
            public int SliderPointCount { get; set; }
            public double AvgSliderPointCount { get; set; }
            public int BèzierSliderCount { get; set; }
            public int CatmullSliderCount { get; set; }
            public int LinearSliderCount { get; set; }
            public int PerfectCicleSliderCount { get; set; }
            public int KickSliderCount { get; set; }
            public int CirclePerfectStackCount { get; set; }
            public int SliderPerfectStackCount { get; set; }
        }
    }
}
