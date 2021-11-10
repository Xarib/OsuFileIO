using OsuFileIO.HitObject.Taiko;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer;

internal class TaikoAnalyzer
{
    private readonly ITaikoAnalysis analysis;

    internal TaikoAnalyzer(ITaikoAnalysis analysis = null)
    {
        this.analysis = analysis;
    }

    internal ITaikoAnalysis Analyze(IReadOnlyBeatmap<TaikoHitObject> beatmap)
    {
        throw new NotImplementedException();
    }
}
