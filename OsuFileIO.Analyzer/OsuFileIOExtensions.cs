using OsuFileIO.HitObject;
using OsuFileIO.HitObject.Catch;
using OsuFileIO.HitObject.Mania;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.HitObject.Taiko;
using OsuFileIO.Analyzer.HitObjectReader;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer;

public static class OsuFileIOExtensions
{
    public static IStdAnalysis Analyze(this IReadOnlyBeatmap<StdHitObject> beatmap, IStdAnalysis analysis = null)
        => new StdAnalyzer(analysis).Analyze(beatmap);

    public static IAnalysis Analyze(this IReadOnlyBeatmap<ManiaHitObject> beatmap, IManiaAnalysis analysis = null)
        => new ManiaAnalyzer(analysis).Analyze(beatmap);

    public static IAnalysis Analyze(this IReadOnlyBeatmap<TaikoHitObject> beatmap, ITaikoAnalysis analysis = null)
        => new TaikoAnalyzer(analysis).Analyze(beatmap);

    public static IAnalysis Analyze(this IReadOnlyBeatmap<CatchHitObject> beatmap, ICatchAnalysis analysis = null)
        => new CatchAnalyzer(analysis).Analyze(beatmap);

    //Leave this here so this is the last overload option
    public static IAnalysis Analyze(this IReadOnlyBeatmap<IHitObject> beatmap, IAnalysis analysis = null)
    {
        switch (beatmap)
        {
            case IReadOnlyBeatmap<StdHitObject> stdBeatmap:
                return stdBeatmap.Analyze((IStdAnalysis)analysis);
            case IReadOnlyBeatmap<ManiaHitObject> maniaBeatmap:
                return maniaBeatmap.Analyze((IManiaAnalysis)analysis);
            case IReadOnlyBeatmap<TaikoHitObject> taikoBeatmap:
                return taikoBeatmap.Analyze((ITaikoAnalysis)analysis);
            case IReadOnlyBeatmap<CatchHitObject> catchBeatmap:
                return catchBeatmap.Analyze((ICatchAnalysis)analysis);
            case null:
                throw new ArgumentNullException(nameof(beatmap));
            default:
                throw new ArgumentException($"Unkown beatmap type");
        }
    }
}
