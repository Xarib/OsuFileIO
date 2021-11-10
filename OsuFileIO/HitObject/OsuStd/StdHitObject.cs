using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.HitObject.OsuStd;

public abstract class StdHitObject : IHitObject
{
    public StdHitObject(Coordinates coordinates, int timeInMs)
    {
        this.Coordinates = coordinates;
        this.TimeInMs = timeInMs;
    }

    public Coordinates Coordinates { get; set; }
    public int TimeInMs { get; set; }
    public Coordinates EndCoordinates { get; protected set; }
}
