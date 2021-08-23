using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject
{
    public class Spinner : IHitObject
    {
        public Coordinates Coordinates { get; set; }
        public int TimeInMs { get; set; }
        public int EndTimeInMs { get; set; }

        public Spinner(Coordinates coordinates, int timeInMs, int endTimeInMs)
        {
            this.Coordinates = coordinates;
            this.TimeInMs = timeInMs;
            this.EndTimeInMs = endTimeInMs;
        }

        public Coordinates GetEndCoordinates()
            => this.Coordinates;

        public int GetEndTime()
            => this.TimeInMs;
    }
}
