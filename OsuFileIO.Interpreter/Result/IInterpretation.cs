using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer.Result
{
    public interface IInterpretation
    {
        public TimeSpan Length { get; set; }
        public double Bpm { get; set; }
        public double BpmMin { get; set; }
        public double BpmMax { get; set; }
    }
}
