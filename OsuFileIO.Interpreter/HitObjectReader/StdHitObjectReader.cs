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
            this.SetSliderVelocity();
        }

        public double MaxTimeBetweenStreamObjects { get; private init; }
        public double MaxTimeBetweenJumps { get; private init; }
        public double SliderVelocity { get; private set; }

        /// <summary>
        /// Reads the next <see cref="IHitObject"/> and set the most current <see cref="TimingPoint"/>
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (this.indexHitObject == this.hitObjects.Count - 1)
                return false;

            this.indexHitObject++;

            this.SetMostCurrentTimingPoint();

            this.SetSliderVelocity();

            return true;
        }

        private void SetSliderVelocity()
        {
            this.SliderVelocity = 100d * (this.difficulty.SliderMultiplier ?? 1d); //100 osuPixels per second is the default slider speed before multipliers
            if (this.CurrentTimingPoint.BeatLength < 0)
            {
                //ChildTimingPoint
                var sliderMultiplier = -100d / this.CurrentTimingPoint.BeatLength;
                this.SliderVelocity = this.SliderVelocity * sliderMultiplier;
            }
        }
    }
}
