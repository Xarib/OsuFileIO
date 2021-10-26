using OsuFileIO.HitObject.Catch;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer
{
    internal class CatchInterpreter
    {
        private readonly ICatchInterpretation source;

        internal CatchInterpreter(ICatchInterpretation source = null)
        {
            this.source = source;
        }

        internal ICatchInterpretation Interpret(IReadOnlyBeatmap<CatchHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
