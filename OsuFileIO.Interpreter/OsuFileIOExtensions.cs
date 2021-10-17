using OsuFileIO.HitObject;
using OsuFileIO.HitObject.Catch;
using OsuFileIO.HitObject.Mania;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.HitObject.Taiko;
using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public static class OsuFileIOExtensions
    {
        public static IOsuStdInterpretation Interpret(this IReadOnlyBeatmap<StdHitObject> beatmap, IOsuStdInterpretation interpretation = null)
        {
            return new OsuStdInterpreter(interpretation).Interpret(beatmap);
        }

        public static IInterpretation Interpret(this IReadOnlyBeatmap<ManiaHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        public static IInterpretation Interpret(this IReadOnlyBeatmap<TaikoHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        public static IInterpretation Interpret(this IReadOnlyBeatmap<CatchHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        //Leave this here so this is the last overload option
        public static IInterpretation Interpret(this IReadOnlyBeatmap<IHitObject> beatmap, IInterpretation interpretation = null)
        {
            switch (beatmap)
            {
                case IReadOnlyBeatmap<StdHitObject> stdBeatmap:
                    return stdBeatmap.Interpret(interpretation);
                case IReadOnlyBeatmap<ManiaHitObject> maniaBeatmap:
                    return maniaBeatmap.Interpret();
                case IReadOnlyBeatmap<TaikoHitObject> taikoBeatmap:
                    return taikoBeatmap.Interpret();
                case IReadOnlyBeatmap<CatchHitObject> catchBeatmap:
                    return catchBeatmap.Interpret();
                case null:
                    throw new ArgumentNullException(nameof(beatmap));
                default:
                    throw new ArgumentException($"Unkown beatmap type");
            }
        }
    }
}
