using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject;

public struct Coordinates : IEquatable<Coordinates>
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public bool Equals(Coordinates other)
    {
        return X == other.X && Y == other.Y;
    }

    public static bool operator ==(Coordinates lhs, Coordinates rhs) => lhs.Equals(rhs);
    public static bool operator !=(Coordinates lhs, Coordinates rhs) => !lhs.Equals(rhs);

    public override int GetHashCode() => (X, Y).GetHashCode();

    public override bool Equals(object obj)
    {
        return obj is Coordinates coordinates && Equals(coordinates);
    }
}
