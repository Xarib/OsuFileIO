using OsuFileIO.HitObject.Taiko;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer
{
    internal class TaikoInterpreter
    {
        private readonly ITaikoInterpretation source;

        internal TaikoInterpreter(ITaikoInterpretation source = null)
        {
            this.source = source;
        }

        internal ITaikoInterpretation Interpret(IReadOnlyBeatmap<TaikoHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
