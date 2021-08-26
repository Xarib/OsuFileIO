using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OsuFileIO.HitObject
{
    public interface IHitObject
    {
        public Coordinates Coordinates { get; set; }
        public int TimeInMs { get; set; }
    }
}
