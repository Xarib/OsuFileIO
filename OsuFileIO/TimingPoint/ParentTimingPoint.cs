using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.TimingPoint
{
    public class ParentTimingPoint
    {
        public int TimeInMs { get; set; }
        public double BeatLength { get; set; }
        public int Meter { get; set; }
        public double SliderMultiplier { get; set; }
    }
}
