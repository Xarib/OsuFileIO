﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public interface IInterpretation
    {
        public TimeSpan Length { get; set; }
        public int HitCircleCount { get; set; }
        public int SliderCount { get; set; }
        public int SpinnerCount { get; set; }
        public double Bpm { get; set; }
        public double BpmMin { get; set; }
        public double BpmMax { get; set; }
        public int DoubleCount { get; set; }
        public int TripletCount { get; set; }
        public int QuadrupletCount { get; set; }
        public int BurstCount { get; set; }
        public int StreamCount { get; set; }
        public int LongStreamCount { get; set; }
        public int DeathStreamCount { get; set; }
        public int LongestStream { get; set; }
    }
}