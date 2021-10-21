using OsuFileIO.HitObject.Mania;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OsuFileIO.Tests")]

namespace OsuFileIO.Interpreter.HitObjectReader
{
    internal class ManiaHitObjectReader : HitObjectReader<ManiaHitObject>
    {
        public ManiaHitObjectReader(Difficulty difficulty, List<TimingPoint> timingPoints, IReadOnlyList<ManiaHitObject> hitObjects)
            : base(difficulty, timingPoints, hitObjects) { }

        internal override bool ReadNext()
        {
            throw new NotImplementedException();
        }
    }
}
