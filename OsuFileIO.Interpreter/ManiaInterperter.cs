using OsuFileIO.HitObject.Mania;
using OsuFileIO.Interpreter.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public class ManiaInterperter
    {
        private readonly IManiaInterpretation source;

        public ManiaInterperter(IManiaInterpretation source = null)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IManiaInterpretation Interpret(IReadOnlyBeatmap<ManiaHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
