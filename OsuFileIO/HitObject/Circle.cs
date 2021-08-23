using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject
{
    public class Circle : IHitObject
    {
        public Coordinates Coordinates { get; set; }
        public int TimeInMs { get; set; }

        public Circle(Coordinates coordinates, int timeInMs)
        {
            this.Coordinates = coordinates;
            this.TimeInMs = timeInMs;
        }

        public Coordinates GetEndCoordinates()
            => this.Coordinates;

        public int GetEndTime()
            => this.TimeInMs;
    }
}
