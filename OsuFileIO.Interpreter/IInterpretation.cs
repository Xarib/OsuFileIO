using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public interface IInterpretation
    {
        public TimeSpan Length { get; set; }
        public int HitCircleCount { get; set; }
        public int SliderCount { get; set; }
        public int SpinnerCount { get; set; }
        public double Bpm { get; set; }
        public double BpmMin { get; set; }
        public double BpmMax { get; set; }
        public int DoubleCount { get; set; }
        public int TrueDoubleCount { get; set; }
        public int TripletCount { get; set; }
        public int TrueTripletCount { get; set; }
        public int QuadrupletCount { get; set; }
        public int TrueQuadrupletCount { get; set; }
        public int BurstCount { get; set; }
        public int StreamCount { get; set; }
        public int LongStreamCount { get; set; }
        public int DeathStreamCount { get; set; }
        public int LongestStream { get; set; }
        public double TotalStreamAlikePixels { get; set; }
        public double TotalSpacedStreamAlikePixels { get; set; }
        public int StreamCutsCount { get; set; }
        public int SlidersInStreamAlike { get; set; }
        public int Jump90DegreesCount { get; set; }
        public int Jump180DegreesCount { get; set; }
    }
}
