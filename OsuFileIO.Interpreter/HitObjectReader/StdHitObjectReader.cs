using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OsuFileIO.Tests")]

namespace OsuFileIO.Interpreter.HitObjectReader
{    
    internal class StdHitObjectReader : HitObjectReader<StdHitObject>
    {
        public StdHitObjectReader(Difficulty difficulty, List<TimingPoint> timingPoints, IReadOnlyList<StdHitObject> hitObjects) : base(difficulty, timingPoints, hitObjects)
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
                throw new ArgumentNullException(nameof(difficulty.CircleSize));


            this.SetValues();

            this.CircleSize = difficulty.CircleSize.Value;
        }

        internal double TimeQuarterBeat { get; private set; }
        internal double TimeHalfBeat { get; private set; }
        internal double TimeEighthOfBeat { get; private set; }

        /// <summary>
        /// Slider verlocity in pixels per beat
        /// </summary>
        internal double SliderVelocity { get; private set; }
        internal StdHitObjectType HitObjectType { get; private set; }
        internal double CircleSize { get; init; }

        private void SetValues()
        {
            this.SetHitObjectType();

            this.SetMostCurrentTimingPoint();

            this.AddCurrentToHistory();

            this.SetSliderVelocity();

            this.SetTimeBetweens();
        }

        /// <summary>
        /// Reads the next <see cref="IHitObject"/> and set the most current <see cref="TimingPoint"/>
        /// </summary>
        /// <returns></returns>
        internal override bool ReadNext()
        {
            if (this.indexHitObject == this.hitObjects.Count - 1)
                return false;

            this.indexHitObject++;

            this.SetValues();

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

    internal enum StdHitObjectType
    {
        Circle = 1,
        Slider = 2,
        Spinner = 3,
    }
}
