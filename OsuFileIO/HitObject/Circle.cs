using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject
{
    public class Circle : IHitObject, IEquatable<Circle>
    {
        public Coordinates Coordinates { get; set; }
        public int TimeInMs { get; set; }

        public Circle(Coordinates coordinates, int timeInMs)
        {
            this.Coordinates = coordinates;
            this.TimeInMs = timeInMs;
        }

        public bool Equals(Circle other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return
                this.Coordinates == other.Coordinates &&
                this.TimeInMs == other.TimeInMs;
        }

        public override bool Equals(object obj)
            => Equals(obj as Circle);

        public override int GetHashCode()
            => (this.Coordinates, this.TimeInMs).GetHashCode();

        public static bool operator ==(Circle lhs, Circle rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                    return true;

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Circle lhs, Circle rhs) => !(lhs == rhs);
    }
}
