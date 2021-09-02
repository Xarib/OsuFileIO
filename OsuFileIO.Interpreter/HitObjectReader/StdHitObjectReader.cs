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
        public StdHitObjectReader(IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects) : base(timingPoints, hitObjects)
        {
        }

        /// <summary>
        /// Reads the next <see cref="IHitObject"/> and set the most current <see cref="TimingPoint"/>
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (this.indexHitObject == this.hitObjects.Count - 1)
                return false;

            this.indexHitObject++;

            //Get Most current Timingpoint
            while (this.hitObjects[this.indexHitObject].TimeInMs > this.CurrentTimingPoint.TimeInMs)
            {
                this.indexTimingPoint++;

                /* 
                 * Checks if list has next timing point and if the time of the timingPoints are equal.
                 * In a situation where two timing points with the same time exist it selects the last one.
                 */
                if (this.indexTimingPoint != this.timingPoints.Count - 1 && this.timingPoints[this.indexTimingPoint + 1].TimeInMs == this.CurrentTimingPoint.TimeInMs)
                    this.indexTimingPoint++;
            }

            return true;
        }
    }
}
