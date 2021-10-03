using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject.OsuStd
{
    public class Spinner : StdHitObject, IEquatable<Spinner>
    {
        public int EndTimeInMs { get; set; }

        public Spinner(Coordinates coordinates, int timeInMs, int endTimeInMs) : base(coordinates, timeInMs)
        {
            this.EndTimeInMs = endTimeInMs;
            this.EndCoordinates = coordinates;
        }

        public bool Equals(Spinner other)
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
                this.EndTimeInMs == other.EndTimeInMs;
        }

        public override bool Equals(object obj)
            => Equals(obj as Spinner);

        public override int GetHashCode()
            => (this.Coordinates, this.TimeInMs, this.EndCoordinates, this.EndTimeInMs).GetHashCode();

        public static bool operator ==(Spinner lhs, Spinner rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                    return true;

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Spinner lhs, Spinner rhs) => !(lhs == rhs);
    }
}
