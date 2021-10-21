using OsuFileIO.HitObject.Catch;
using OsuFileIO.Interpreter.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public class CatchInterpreter
    {
        private readonly ICatchInterpretation source;

        public CatchInterpreter(ICatchInterpretation source = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public ICatchInterpretation Interpret(IReadOnlyBeatmap<CatchHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
