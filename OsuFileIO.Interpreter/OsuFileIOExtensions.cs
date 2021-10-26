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

namespace OsuFileIO.Analyzer
{
    public static class OsuFileIOExtensions
    {
        public static IStdInterpretation Interpret(this IReadOnlyBeatmap<StdHitObject> beatmap, IStdInterpretation interpretation = null)
            => new StdInterpreter(interpretation).Interpret(beatmap);

        public static IInterpretation Interpret(this IReadOnlyBeatmap<ManiaHitObject> beatmap, IManiaInterpretation interpretation = null)
            => new ManiaInterperter(interpretation).Interpret(beatmap);

        public static IInterpretation Interpret(this IReadOnlyBeatmap<TaikoHitObject> beatmap, ITaikoInterpretation interpretation = null)
            => new TaikoInterpreter(interpretation).Interpret(beatmap);

        public static IInterpretation Interpret(this IReadOnlyBeatmap<CatchHitObject> beatmap, ICatchInterpretation interpretation = null)
            => new CatchInterpreter(interpretation).Interpret(beatmap);

        //Leave this here so this is the last overload option
        public static IInterpretation Interpret(this IReadOnlyBeatmap<IHitObject> beatmap, IInterpretation interpretation = null)
        {
            switch (beatmap)
            {
                case IReadOnlyBeatmap<StdHitObject> stdBeatmap:
                    return stdBeatmap.Interpret((IStdInterpretation)interpretation);
                case IReadOnlyBeatmap<ManiaHitObject> maniaBeatmap:
                    return maniaBeatmap.Interpret((IManiaInterpretation)interpretation);
                case IReadOnlyBeatmap<TaikoHitObject> taikoBeatmap:
                    return taikoBeatmap.Interpret((ITaikoInterpretation)interpretation);
                case IReadOnlyBeatmap<CatchHitObject> catchBeatmap:
                    return catchBeatmap.Interpret((ICatchInterpretation)interpretation);
                case null:
                    throw new ArgumentNullException(nameof(beatmap));
                default:
                    throw new ArgumentException($"Unkown beatmap type");
            }
        }
    }
}
