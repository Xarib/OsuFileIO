using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject
{
    public class Slider : IHitObject
    {
        public Coordinates Coordinates { get; set; }
        public int TimeInMs { get; set; }
        public int EndTimeInMs { get; set; }
        public double Length { get; set; }
        public List<Coordinates> SliderCoordinates { get; set; }

        public Slider(Coordinates coordinates, int timeInMs, int endTimeInMs, double length, List<Coordinates> sliderCoordinates)
        {
            this.Coordinates = coordinates;
            this.TimeInMs = timeInMs;
            this.EndTimeInMs = endTimeInMs;
            this.Length = length;
            this.SliderCoordinates = sliderCoordinates;
        }

        public Coordinates GetEndCoordinates()
            => this.Coordinates;

        public int GetEndTime()
            => this.TimeInMs;
    }
}
