using OsuFileIO.HitObject.Catch;
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
    internal class CatchHitObjectReader : HitObjectReader<CatchHitObject>
    {
        internal CatchHitObjectReader(Difficulty difficulty, List<TimingPoint> timingPoints, IReadOnlyList<CatchHitObject> hitObjects)
            : base(difficulty, timingPoints, hitObjects) { }

        internal override bool ReadNext()
        {
            throw new NotImplementedException();
        }
    }
}
