using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject.OsuStd;

public class Slider : StdHitObject, IEquatable<Slider>
{
    public double Length { get; set; }
    public List<Coordinates> SliderCoordinates { get; set; }
    public CurveType CurveType { get; set; }
    public int Slides { get; set; }
    public double TravelLenth { get; set; }

    public Slider(Coordinates coordinates, int timeInMs, List<Coordinates> sliderCoordinates, double length, CurveType curveType, int slides) : base(coordinates, timeInMs)
    {
        this.Coordinates = coordinates;
        this.TimeInMs = timeInMs;
        this.SliderCoordinates = sliderCoordinates;
        this.Length = length;
        this.CurveType = curveType;
        this.TravelLenth = length * slides;
        this.Slides = slides;

        if (slides % 2 == 1)
        {
            this.EndCoordinates = coordinates;
        }
        else
        {
            this.EndCoordinates = sliderCoordinates.Last();
        }
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
            this.SliderCoordinates.SequenceEqual(other.SliderCoordinates) &&
            this.CurveType == other.CurveType;
    }

    public override bool Equals(object obj)
        => Equals(obj as Slider);

    public override int GetHashCode()
        => (this.Coordinates, this.TimeInMs, this.SliderCoordinates, this.Length, this.CurveType, this.Slides, this.TravelLenth, this.EndCoordinates).GetHashCode();

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

public enum CurveType
{
    Bézier = 'B',
    CentripetalCatmullRom = 'C',
    Linear = 'L',
    PerfectCircle = 'P',
}
