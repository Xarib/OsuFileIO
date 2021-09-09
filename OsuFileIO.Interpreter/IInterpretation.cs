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
    }
}
