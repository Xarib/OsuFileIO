using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer.Result;

public interface IStdAnalysis : IAnalysis
{
    public int HitCircleCount { get; set; }
    public int SliderCount { get; set; }
    public int SpinnerCount { get; set; }
    public int DoubleCount { get; set; }
    public int StandaloneDoubleCount { get; set; }
    public int TripletCount { get; set; }
    public int StandaloneTripletCount { get; set; }
    public int QuadrupletCount { get; set; }
    public int StandaloneQuadrupletCount { get; set; }
    public int BurstCount { get; set; }
    public int StreamCount { get; set; }
    public int LongStreamCount { get; set; }
    public int DeathStreamCount { get; set; }
    public int LongestStream { get; set; }
    public double TotalStreamAlikePixels { get; set; }
    public double TotalSpacedStreamAlikePixels { get; set; }
    public int StreamCutsCount { get; set; }
    public int SlidersInStreamAlike { get; set; }
    public int Jump90DegreesCount { get; set; }
    public int Jump180DegreesCount { get; set; }
    public double TotalJumpPixels { get; set; }
    public int CrossScreenJumpCount { get; set; }
    public double TotalSliderLength { get; set; }
    public int BèzierSliderCount { get; set; }
    public int CatmullSliderCount { get; set; }
    public int LinearSliderCount { get; set; }
    public int PerfectCicleSliderCount { get; set; }
    public int SliderPointCount { get; set; }
    public double AvgSliderPointCount { get; set; }
    public int KickSliderCount { get; set; }
    public double AvgFasterSliderSpeed { get; set; }
    public double SliderSpeedDifference { get; set; }
    public int CirclePerfectStackCount { get; set; }
    public int SliderPerfectStackCount { get; set; }
    public int UniqueDistancesCount { get; set; }
}
