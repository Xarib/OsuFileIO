using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Interpreter
{
    public class OsuStdInterpreter
    {
        private readonly IInterpretation source;

        public OsuStdInterpreter(IInterpretation source)
        {
            this.source = source ?? new OsuStdInterpretation();
        }

        public void Interpret(OsuStdFile beatmap)
        {
            var reader = new StdHitObjectReader(beatmap.Difficulty, beatmap.TimingPoints, beatmap.HitObjects);

            //while (reader.ReadNext())
            //{

            //}
        }

        private class OsuStdInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
        }
    }
}
