using OsuFileIO.HitObject.Taiko;
using OsuFileIO.Interpreter.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    internal class TaikoInterpreter
    {
        private readonly ITaikoInterpretation source;

        internal TaikoInterpreter(ITaikoInterpretation source = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        internal ITaikoInterpretation Interpret(IReadOnlyBeatmap<TaikoHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
