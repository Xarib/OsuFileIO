using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OsuFileIO.Interpreter.HitObjectReader.StdHitObjectReader;

namespace OsuFileIO.Tests.OsuFileIO.Interpreter.HitObjectReader
{
    [TestClass]
    public class StdHitObjectReaderTests
    {
        [TestMethod]
        public void SetMostCurrentTimingPoint_SameTime_ReturnsMostCurrentTimingPoint()
        {
            //Arrange
            var timingPoints = new List<TimingPoint>
            {
                new TimingPoint
                {
                    TimeInMs = 25,
                },
                new TimingPoint
                {
                    BeatLength = 100,
                    TimeInMs = 50,
                },
                new TimingPoint
                {
                    BeatLength = 200,
                    TimeInMs = 50,
                },
                new TimingPoint
                {
                    BeatLength = 200,
                    TimeInMs = 55,
                },
            };

            var hitObjects = new List<IHitObject>
            {
                new Circle(new Coordinates(), 30),
                new Circle(new Coordinates(), 50),
            };

            var difficulty = new Difficulty();

            //Act
            var reader = new StdHitObjectReader(difficulty, timingPoints, hitObjects);

            do
            {
                ;
            } while (reader.ReadNext());

            //Assert
            Assert.AreEqual(hitObjects[1].TimeInMs, reader.CurrentTimingPoint.TimeInMs, "Expected a timing point that has a smaller or equal time of the last object");
            Assert.AreEqual(timingPoints[2].BeatLength, reader.CurrentTimingPoint.BeatLength, "Expected the most current timingPoint");
        }

        [TestMethod]
        public void SetMostCurrentTimingPoint_SameTimeAtEnd_ReturnsMostCurrentTimingPoint()
        {
            //Arrange
            var timingPoints = new List<TimingPoint>
            {
                new TimingPoint
                {
                    TimeInMs = 25,
                },
                new TimingPoint
                {
                    BeatLength = 100,
                    TimeInMs = 50,
                },
                new TimingPoint
                {
                    BeatLength = 200,
                    TimeInMs = 50,
                },
            };

            var hitObjects = new List<IHitObject>
            {
                new Circle(new Coordinates(), 30),
                new Circle(new Coordinates(), 50),
            };

            var difficulty = new Difficulty();

            //Act
            var reader = new StdHitObjectReader(difficulty, timingPoints, hitObjects);

            do
            {
                ;
            } while (reader.ReadNext());

            //Assert
            Assert.AreEqual(hitObjects[1].TimeInMs, reader.CurrentTimingPoint.TimeInMs, "Expected a timing point that has a smaller or equal time of the last object");
            Assert.AreEqual(timingPoints[2].BeatLength, reader.CurrentTimingPoint.BeatLength, "Expected the most current timingPoint");
        }

        [TestMethod]
        [DataRow(1, "1266,279.06976744186,4,1,9,90,1,0", 100d * 1d)]
        [DataRow(1.6, "1266,279.06976744186,4,1,9,90,1,0", 100d * 1.6d)]
        [DataRow(0.7, "34125,-100,4,1,0,87,0,0", 100d * 0.7d * 1d)]
        [DataRow(0.7, "40472,-83.3333333333333,4,1,0,100,0,0", 100d * 0.7d * 1.2d)]
        public void ReadNext_SliderMultiplierNotNull_ReturnsSliderVelocity(double sliderMultiplier, string timingPoint, double expected)
        {
            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("StackLeniency: 0.7");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[Metadata]");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("SliderMultiplier:" + sliderMultiplier.ToString());
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine(timingPoint);
            writer.WriteLine("[HitObjects]");
            writer.WriteLine("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:");
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderFactory(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects);

            //Assert
            Assert.AreEqual(expected, Math.Round(reader.SliderVelocity, 2), $"Expected to calculate {nameof(reader.SliderVelocity)} correctly");
        }

        [TestMethod]
        [DataRow("1266,279.06976744186,4,1,9,90,1,0", 100d * 1d)]
        [DataRow("34125,-100,4,1,0,87,0,0", 100d * 1d * 1d)]
        [DataRow("40472,-83.3333333333333,4,1,0,100,0,0", 100d * 1d * 1.2d)]
        public void ReadNext_SliderMultiplierNull_ReturnsSliderVelocity(string timingPoint, double expected)
        {
            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("StackLeniency: 0.7");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[Metadata]");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine(timingPoint);
            writer.WriteLine("[HitObjects]");
            writer.WriteLine("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:");
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderFactory(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects);
        }

        [TestMethod]
        [DataRow("255,184,3664,69,4,3:1:0:0:", StdHitObjectType.Circle)]
        [DataRow("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:", StdHitObjectType.Slider)]
        [DataRow("256,192,126433,12,4,129202,3:2:0:0:", StdHitObjectType.Spinner)]
        public void ReadNext_EmptyFileWithHitObject_ReturnsHitObjectType(string hitObject, StdHitObjectType hitObjectType)
        {
            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("StackLeniency: 0.7");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[Metadata]");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
            writer.WriteLine("[HitObjects]");
            writer.WriteLine(hitObject);
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderFactory(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects);

            //Assert
            Assert.AreEqual(hitObjectType, reader.HitObjectType, "Expected to get the correct type for the given hitObject string");
        }
    }
}
