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

            Dictionary<double, int> totalTimeByBpm = new();
            TimingPoint lastRedTimingPoint = null;
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

                if (reader.CurrentTimingPoint is not InheritedPoint)
                {
                    if (lastRedTimingPoint is null)
                    {
                        totalTimeByBpm.Add(reader.CurrentTimingPoint.BeatLength, 0);
                        lastRedTimingPoint = reader.CurrentTimingPoint;
                    }
                    else
                    {
                        var timeDurationLastPoint = reader.CurrentTimingPoint.TimeInMs - lastRedTimingPoint.TimeInMs;
                        totalTimeByBpm[lastRedTimingPoint.BeatLength] += timeDurationLastPoint;

                        if (!totalTimeByBpm.ContainsKey(reader.CurrentTimingPoint.BeatLength))
                            totalTimeByBpm.Add(reader.CurrentTimingPoint.BeatLength, 0);

                        lastRedTimingPoint = reader.CurrentTimingPoint;
                    }
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

            totalTimeByBpm[lastRedTimingPoint.BeatLength] += Convert.ToInt32(this.source.Length.TotalMilliseconds - lastRedTimingPoint.TimeInMs);

            var longestTimeForBpms = 0;
            this.source.BpmMax = int.MinValue;
            this.source.BpmMin = int.MaxValue;
            foreach (var item in totalTimeByBpm)
            {
                var currentBpm = Convert.ToInt32(1 / item.Key * 60000d);
                if (item.Value > longestTimeForBpms)
                {
                    longestTimeForBpms = item.Value;
                    this.source.Bpm = currentBpm;
                }

                if (currentBpm > this.source.BpmMax)
                    this.source.BpmMax = currentBpm;

                if (currentBpm < this.source.BpmMin)
                    this.source.BpmMin = currentBpm;
            }
        }

        private class OsuStdInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
            public int HitCircleCount { get; set; }
            public int SliderCount { get; set; }
            public int SpinnerCount { get; set; }
            public double Bpm { get; set; }
            public double BpmMin { get; set; }
            public double BpmMax { get; set; }
        }
    }
}
