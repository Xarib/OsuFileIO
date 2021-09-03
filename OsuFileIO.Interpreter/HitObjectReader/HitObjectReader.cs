using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter.HitObjectReader
{
    public abstract class HitObjectReader
    {
        protected readonly Difficulty difficulty;
        protected readonly IList<TimingPoint> timingPoints;
        protected readonly IList<IHitObject> hitObjects;
        protected int indexHitObject;
        protected int indexTimingPoint;

        public TimingPoint CurrentTimingPoint { get => this.timingPoints[this.indexTimingPoint]; }
        public IHitObject CurrentHitObject { get => this.hitObjects[this.indexHitObject]; }

        public HitObjectReader(Difficulty difficulty, IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects)
        {
            this.difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));
            this.timingPoints = timingPoints ?? throw new ArgumentNullException(nameof(timingPoints));
            this.hitObjects = hitObjects ?? throw new ArgumentNullException(nameof(hitObjects));
        }

        public abstract bool ReadNext();

        public TimingPoint GetTimingPoint(int offsetFromCurrent)
        {
            var indexAfterOffset = this.indexTimingPoint + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.timingPoints.Count)
                return null;

            return this.timingPoints[indexAfterOffset];
        }

        public IHitObject GetHitObject(int offsetFromCurrent)
        {
            var indexAfterOffset = this.indexHitObject + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.hitObjects.Count)
                return null;

            return this.hitObjects[indexAfterOffset];
        }

        protected void SetMostCurrentTimingPoint()
        {
            var hasChanged = false;
            while (this.indexTimingPoint < this.timingPoints.Count - 1 && this.CurrentTimingPoint.TimeInMs <= this.CurrentHitObject.TimeInMs)
            {
                this.indexTimingPoint++;
                hasChanged = true;
            }

            if (hasChanged && this.CurrentTimingPoint.TimeInMs != this.CurrentHitObject.TimeInMs)
                this.indexTimingPoint--;
        }
    }
}
