using OsuFileIO.HitObject.Mania;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer
{
    internal class ManiaInterperter
    {
        private readonly IManiaInterpretation source;

        internal ManiaInterperter(IManiaInterpretation source = null)
        {
            this.source = source;
        }

        internal IManiaInterpretation Interpret(IReadOnlyBeatmap<ManiaHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
