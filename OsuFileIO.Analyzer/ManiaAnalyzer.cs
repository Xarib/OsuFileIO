using OsuFileIO.HitObject.Mania;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer;

internal class ManiaAnalyzer
{
    private readonly IManiaAnalysis analysis;

    internal ManiaAnalyzer(IManiaAnalysis analysis = null)
    {
        this.analysis = analysis;
    }

    internal IManiaAnalysis Analyze(IReadOnlyBeatmap<ManiaHitObject> beatmap)
    {
        throw new NotImplementedException();
    }
}
