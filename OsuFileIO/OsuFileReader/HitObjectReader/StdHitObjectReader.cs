using OsuFileIO.HitObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader.HitObjectReader
{
    public class StdHitObjectReader : HitObjectReader
    {
        public StdHitObjectReader(IList<TimingPoint> timingPoints, IList<IHitObject> hitObjects) : base(timingPoints, hitObjects)
        {
        }

        public override void ReadNext()
        {
            this.index++;

            //while (this.hitObjects[this.index].TimeInMs )

        }
    }
}
