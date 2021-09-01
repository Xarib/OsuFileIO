using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject
{
    public class Slider : IHitObject, IEquatable<Slider>
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

        public bool Equals(Slider other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return
                this.Coordinates == other.Coordinates &&
                this.TimeInMs == other.TimeInMs &&
                this.Length == other.Length &&
                this.SliderCoordinates.SequenceEqual(other.SliderCoordinates);
        }

        public override bool Equals(object obj)
            => Equals(obj as Slider);

        public override int GetHashCode()
            => (this.Coordinates, this.TimeInMs).GetHashCode();

        public static bool operator ==(Slider lhs, Slider rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                    return true;

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Slider lhs, Slider rhs) => !(lhs == rhs);
    }
}
