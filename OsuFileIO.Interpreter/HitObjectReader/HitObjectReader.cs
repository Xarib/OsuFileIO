using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter.HitObjectReader
{
    internal abstract class HitObjectReader<THitObject> where THitObject : IHitObject
    {
        protected readonly Difficulty difficulty;
        protected readonly List<TimingPoint> timingPoints;
        protected readonly IReadOnlyList<THitObject> hitObjects;
        protected int indexHitObject;
        protected int indexTimingPoint;

        public TimingPoint CurrentTimingPoint { get => this.timingPoints[this.indexTimingPoint]; }
        public THitObject CurrentHitObject { get => this.hitObjects[this.indexHitObject]; }

        private List<(TimingPoint, THitObject)> History { get; init; }

        public HitObjectReader(Difficulty difficulty, List<TimingPoint> timingPoints, IReadOnlyList<THitObject> hitObjects)
        {
            this.difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));
            this.timingPoints = timingPoints ?? throw new ArgumentNullException(nameof(timingPoints));
            this.History = new List<(TimingPoint, THitObject)>();
            this.hitObjects = hitObjects;

            //if (hitObjects is null)
            //{
            //    throw new ArgumentNullException(nameof(hitObjects));
            //}
            //else
            //{
            //    //TODO look how much of an isssue this is.
            //    this.hitObjects = hitObjects.Cast<THitObject>().ToList();
            //}
        }

        public abstract bool ReadNext();

        public TimingPoint GetTimingPointFromOffsetOrNull(int offsetFromCurrent)
        {
            var indexAfterOffset = this.indexTimingPoint + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.timingPoints.Count)
                return null;

            return this.timingPoints[indexAfterOffset];
        }

        public THitObject GetHitObjectFromOffsetOrNull(int offsetFromCurrent)
        {
            var indexAfterOffset = this.indexHitObject + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= this.hitObjects.Count)
                return default;

            return this.hitObjects[indexAfterOffset];
        }

        public (TimingPoint timingPoint, THitObject hitObject)? GetHistoryEntryOrNull(int offsetFromCurrent)
        {
            var index = this.History.Count + offsetFromCurrent - 1;

            if (index < 0 || index == this.History.Count)
                return null;

            return this.History[index];
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
