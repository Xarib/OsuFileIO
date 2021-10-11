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
        public static IOsuStdInterpretation Interpret(this IReadOnlyBeatmap<StdHitObject> beatmap)
        {
            return new OsuStdInterpreter().Interpret(beatmap);
        }

        public static IOsuStdInterpretation Interpret(this IReadOnlyBeatmap<ManiaHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        public static IOsuStdInterpretation Interpret(this IReadOnlyBeatmap<TaikoHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        public static IOsuStdInterpretation Interpret(this IReadOnlyBeatmap<CatchHitObject> beatmap)
        {
            throw new NotImplementedException();
        }

        //Leave this here so this is the last overload option
        public static object Interpret(this IReadOnlyBeatmap<IHitObject> beatmap)
        {
            switch (beatmap)
            {
                case IReadOnlyBeatmap<StdHitObject> stdBeatmap:
                    return Interpret(stdBeatmap);
                case IReadOnlyBeatmap<ManiaHitObject> maniaBeatmap:
                    return Interpret(maniaBeatmap);
                case IReadOnlyBeatmap<TaikoHitObject> taikoBeatmap:
                    return Interpret(taikoBeatmap);
                case IReadOnlyBeatmap<CatchHitObject> catchBeatmap:
                    return Interpret(catchBeatmap);
                case null:
                    throw new ArgumentNullException(nameof(beatmap));
                default:
                    throw new ArgumentException($"Unkown beatmap type");
            }
        }
    }
}
