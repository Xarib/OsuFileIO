using OsuFileIO.HitObject;
using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            do
            {
                switch (reader.HitObjectType)
                {
                    case StdHitObjectType.Circle:
                        this.source.HitCircleCount++;
                        break;
                    case StdHitObjectType.Slider:
                        this.source.SliderCount++;
                        break;
                    case StdHitObjectType.Spinner:
                        this.source.SpinnerCount++;
                        break;
                    default:
                        throw new InvalidEnumArgumentException($"Unimplemented enum {reader.HitObjectType}");
                }
            } while (reader.ReadNext());

            switch (reader.HitObjectType)
            {
                case StdHitObjectType.Circle:
                    this.source.Length = TimeSpan.FromMilliseconds(reader.CurrentHitObject.TimeInMs);
                    break;
                case StdHitObjectType.Slider:
                    var slider = reader.CurrentHitObject as Slider;
                    double sliderTime = slider.Length / reader.SliderVelocity * reader.CurrentTimingPoint.BeatLength;
                    this.source.Length = TimeSpan.FromMilliseconds(sliderTime + slider.TimeInMs);
                    break;
                case StdHitObjectType.Spinner:
                    var spinner = reader.CurrentHitObject as Spinner;
                    this.source.Length = TimeSpan.FromMilliseconds(spinner.EndTimeInMs);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unimplemented enum {reader.HitObjectType}");
            }
        }

        private class OsuStdInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
            public int HitCircleCount { get; set; }
            public int SliderCount { get; set; }
            public int SpinnerCount { get; set; }
        }
    }
}
