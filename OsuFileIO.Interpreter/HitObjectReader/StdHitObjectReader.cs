using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter.HitObjectReader
{
    public class StdHitObjectReader : HitObjectReader
    {
        public StdHitObjectReader(Difficulty difficulty, IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects) : base(difficulty, timingPoints, hitObjects)
        {
            if (difficulty is null)
                throw new ArgumentNullException(nameof(difficulty));

            if (timingPoints is null)
                throw new ArgumentNullException(nameof(timingPoints));

            if (hitObjects is null)
                throw new ArgumentNullException(nameof(hitObjects));

            if (timingPoints.Count == 0)
                throw new ArgumentException("Map has to have timingPoints");

            if (hitObjects.Count == 0)
                throw new ArgumentException("Map has to have hit objects");

            if (difficulty.CircleSize is null)
                throw new ArgumentNullException("Difficulty cannot be null");


            this.SetHitObjectType();

            this.SetMostCurrentTimingPoint();

            this.AddCurrentToHistory();

            this.SetSliderVelocity();

            this.SetTimeBetweens();

            this.CircleSize = difficulty.CircleSize.Value;
        }

        public double TimeQuarterBeat { get; private set; }
        public double TimeHalfBeat { get; private set; }
        public double TimeEighthOfBeat { get; private set; }

        /// <summary>
        /// Slider verlocity in pixels per beat
        /// </summary>
        public double SliderVelocity { get; private set; }
        public StdHitObjectType HitObjectType { get; private set; }
        public double CircleSize { get; init; }

        /// <summary>
        /// Reads the next <see cref="IHitObject"/> and set the most current <see cref="TimingPoint"/>
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (this.indexHitObject == this.hitObjects.Count - 1)
                return false;

            this.indexHitObject++;

            this.SetHitObjectType();

            this.SetMostCurrentTimingPoint();

            this.AddCurrentToHistory();

            this.SetSliderVelocity();

            this.SetTimeBetweens();

            return true;
        }

        private void SetSliderVelocity()
        {
            this.SliderVelocity = 100d * (this.difficulty.SliderMultiplier ?? 1d); //100 osuPixels per second is the default slider speed before multipliers

            if (this.CurrentTimingPoint is InheritedPoint inheritedPoint)
            {
                this.SliderVelocity *= inheritedPoint.VelocityMultiplier;
            }
        }

        private void SetHitObjectType()
        {
            if (this.CurrentHitObject is Circle)
            {
                this.HitObjectType = StdHitObjectType.Circle;
            }
            else if (this.CurrentHitObject is Slider)
            {
                this.HitObjectType = StdHitObjectType.Slider;
            }
            else
            {
                this.HitObjectType = StdHitObjectType.Spinner;
            }
        }

        private void SetTimeBetweens()
        {
            this.TimeQuarterBeat = this.CurrentTimingPoint.BeatLength / 4;
            this.TimeHalfBeat = this.CurrentTimingPoint.BeatLength / 2;
            this.TimeEighthOfBeat = this.CurrentTimingPoint.BeatLength / 8;
        }
    }

    public enum StdHitObjectType
    {
        Circle = 1,
        Slider = 2,
        Spinner = 3,
    }
}
