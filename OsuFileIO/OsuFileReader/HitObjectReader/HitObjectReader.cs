using OsuFileIO.HitObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader.HitObjectReader
{
    public abstract class HitObjectReader
    {
        protected readonly IList<TimingPoint> timingPoints;
        protected readonly IList<IHitObject> hitObjects;
        protected int index;

        public TimingPoint CurrentTimingPoint { get; init; }
        public double MaxTimeBetweenStreamObjects { get; init; }
        public double MaxTimeBetweenJumps { get; init; }
        public double SliderBaseLength { get; init; }

        public HitObjectReader(IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects)
        {
            this.timingPoints = timingPoints ?? throw new ArgumentNullException(nameof(timingPoints));
            this.hitObjects = hitObjects ?? throw new ArgumentNullException(nameof(hitObjects));
        }

        public abstract void ReadNext();

        public IHitObject GetHitObject(int offsetFromCurrent)
        {
            var indexAfterOffset = this.index + offsetFromCurrent;

            if (indexAfterOffset < 0 || indexAfterOffset >= hitObjects.Count)
                return null;

            return this.hitObjects[indexAfterOffset];
        }
    }
}
