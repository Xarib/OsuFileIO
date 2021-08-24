using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader.HitObjectReader
{
    public class TimingPoint
    {
        public int TimeInMs { get; set; }
        public double BeatLength { get; set; }
        public int Meter { get; set; }
    }
}
