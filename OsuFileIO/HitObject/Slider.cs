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
        public double Length { get; set; }
        public List<Coordinates> SliderCoordinates { get; set; }

        public Slider(Coordinates coordinates, int timeInMs, List<Coordinates> sliderCoordinates, double length)
        {
            this.Coordinates = coordinates;
            this.TimeInMs = timeInMs;
            this.SliderCoordinates = sliderCoordinates;
            this.Length = length;
        }
    }
}
