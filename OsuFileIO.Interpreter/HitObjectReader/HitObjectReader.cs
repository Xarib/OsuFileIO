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

        private List<(TimingPoint, IHitObject)> History { get; init; }

        public HitObjectReader(Difficulty difficulty, IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects)
        {
            this.difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));
            this.timingPoints = timingPoints ?? throw new ArgumentNullException(nameof(timingPoints));
            this.hitObjects = hitObjects ?? throw new ArgumentNullException(nameof(hitObjects));
            this.History = new List<(TimingPoint, IHitObject)>();
        }

        public abstract bool ReadNext();

        public TimingPoint GetTimingPointFromOffsetOrNull(int offsetFromCurrent)
        {
            var indexAfterOffset = this.indexTimingPoint + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.timingPoints.Count)
                return null;

            return this.timingPoints[indexAfterOffset];
        }

        public THitObject GetHitObjectFromOffsetOrNull<THitObject>(int offsetFromCurrent) where THitObject : IHitObject
        {
            var indexAfterOffset = this.indexHitObject + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.hitObjects.Count)
                return default;

            return (THitObject)this.hitObjects[indexAfterOffset];
        }

        public (TimingPoint, THitObject)? GetHistoryEntryOrNull<THitObject>(int offsetFromCurrent) where THitObject : IHitObject
        {
            var index = this.History.Count + offsetFromCurrent - 1;

            if (index < 0 || index == this.History.Count)
                return null;

            var item = this.History[index];

            return (item.Item1, (THitObject)item.Item2);
        }

        protected void SetMostCurrentTimingPoint()
        {
            var hasChanged = false;
            while (this.indexTimingPoint < this.timingPoints.Count - 1 && this.CurrentTimingPoint.TimeInMs <= this.CurrentHitObject.TimeInMs)
            {
                this.indexTimingPoint++;
                hasChanged = true;
            }

            if (this.indexTimingPoint == this.timingPoints.Count - 1 && this.CurrentTimingPoint.TimeInMs <= this.CurrentHitObject.TimeInMs)
                return;

            if (hasChanged)
                this.indexTimingPoint--;
        }

        protected void AddCurrentToHistory()
            => this.History.Add((this.CurrentTimingPoint, this.CurrentHitObject));
    }
}
