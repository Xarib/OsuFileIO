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
    internal class CatchAnalyzer
    {
        private readonly ICatchAnalysis analysis;

        internal CatchAnalyzer(ICatchAnalysis analysis = null)
        {
            this.analysis = analysis;
        }

        internal ICatchAnalysis Analyze(IReadOnlyBeatmap<CatchHitObject> beatmap)
        {
            throw new NotImplementedException();
        }
    }
}
