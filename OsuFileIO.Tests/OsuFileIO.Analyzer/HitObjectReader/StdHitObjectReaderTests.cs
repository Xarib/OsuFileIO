using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.Analyzer.HitObjectReader;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsuFileIO.Tests.OsuFileIO.Analyzer.HitObjectReader
{
    [TestClass]
    public class StdHitObjectReaderTests
    {
        [TestMethod]
        [DataRow(new int[] { -10 }, new int[] { 10 }, 0)]
        [DataRow(new int[] { 10, 11 }, new int[] { 10 }, 0)]
        [DataRow(new int[] { 10, 10 }, new int[] { 10 }, 1)]
        [DataRow(new int[] { 10, 10, 10, 11 }, new int[] { 10 }, 2)]
        [DataRow(new int[] { 25, 50, 50 }, new int[] { 30, 50 }, 2)]
        [DataRow(new int[] { 25, 50, 50, 50 }, new int[] { 30, 50 }, 3)]
        [DataRow(new int[] { 25, 50, 50, 55 }, new int[] { 30, 50 }, 2)]
        [DataRow(new int[] { 10, 20 }, new int[] { 33 }, 1)]
        [DataRow(new int[] { 10, 10, 20 }, new int[] { 33 }, 2)]
        [DataRow(new int[] { 10, 20, 20 }, new int[] { 33 }, 2)]
        [DataRow(new int[] { 10, 20, 20 }, new int[] { 17 }, 0)]
        [DataRow(new int[] { 10, 15, 20 }, new int[] { 17 }, 1)]
        public void SetMostCurrentTimingPoint_MultipleTimingpoints_ReturnsMostCurrentTimingPoint(int[] timingPointTimes, int[] hitObjectTimes, int expectedTimingPointIndex)
        {
            //Arrange
            var difficulty = new Difficulty();
            difficulty.CircleSize = 4;

            var timingPoints = new List<TimingPoint>();
            for (int i = 0; i < timingPointTimes.Length; i++)
            {
                timingPoints.Add(new TimingPoint
                {
                    BeatLength = i,
                    TimeInMs = timingPointTimes[i],
                });
            }

            var hitObjects = new List<IHitObject>();
            for (int i = 0; i < hitObjectTimes.Length; i++)
            {
                hitObjects.Add(new Circle(new Coordinates(), hitObjectTimes[i]));
            }

            var reader = new StdHitObjectReader(difficulty, timingPoints, hitObjects.Cast<StdHitObject>().ToList());

            //Act
            do
            {
            } while (reader.ReadNext());

            //Arrange
            var expectedTimingpoint = timingPoints[expectedTimingPointIndex];
            Assert.AreEqual(expectedTimingpoint.BeatLength, reader.CurrentTimingPoint.BeatLength, "Expected to get the expected timingpoint with the correct index");
            Assert.AreEqual(expectedTimingpoint.TimeInMs, reader.CurrentTimingPoint.TimeInMs, "Expected to get the expected timingpoint time");
            Assert.IsTrue(reader.CurrentTimingPoint.TimeInMs <= reader.CurrentHitObject.TimeInMs, "Expected no timingpoint where the time is bigger than hit object time");
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
            writer.WriteLine("CircleSize:4");
            writer.WriteLine("SliderMultiplier:" + sliderMultiplier.ToString());
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine("1266,10,4,1,9,90,1,0");
            writer.WriteLine(timingPoint);
            writer.WriteLine("[HitObjects]");
            writer.WriteLine("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:");
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderBuilder(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects.Cast<StdHitObject>().ToList());

            //Assert
            Assert.AreEqual(expected, Math.Round(reader.SliderVelocity, 2), $"Expected to calculate {nameof(reader.SliderVelocity)} correctly");
        }

        [TestMethod]
        [DataRow("255,184,3664,69,4,3:1:0:0:", 1)]
        [DataRow("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:", 2)]
        [DataRow("256,192,126433,12,4,129202,3:2:0:0:", 3)]
        public void ReadNext_EmptyFileWithHitObject_ReturnsHitObjectType(string hitObject, int type)
        {
            var hitObjectType = (StdHitObjectType)type;

            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("StackLeniency: 0.7");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[Metadata]");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("CircleSize:4");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
            writer.WriteLine("[HitObjects]");
            writer.WriteLine(hitObject);
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderBuilder(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects.Cast<StdHitObject>().ToList());

            //Assert
            Assert.AreEqual(hitObjectType, reader.HitObjectType, "Expected to get the correct type for the given hitObject string");
        }

        [TestMethod]
        [DataRow(400, 200, 100)]//150Bmp
        [DataRow(600, 300, 150)]//150Bmp
        public void ReadNext_Bpms_ReturnsMaxTimes(double bmp, double timeOneTwos, double timeStreams)
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
            writer.WriteLine("CircleSize:4");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine($"1266,{bmp},4,1,9,90,1,0");
            writer.WriteLine("[HitObjects]");
            writer.WriteLine("255,184,3664,69,4,3:1:0:0:");
            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderBuilder(stream).Build();

            var file = fileReader.ReadFile();

            //Act
            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects.Cast<StdHitObject>().ToList());

            //Assert
            Assert.AreEqual(timeOneTwos, reader.TimeHalfBeat, "Expected to calculate time between 1-2 jumps");
            Assert.AreEqual(timeStreams, reader.TimeQuarterBeat, "Expected to calculate time between stream objects");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-8)]
        public void GetHistoryEntry_HitObjects_OffsetFromGetHitObjectAndGetHistoryShouldMatch(int offset)
        {
            if (offset > 0)
                throw new ArgumentException("Offset cannot be bigger than 0");

            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("StackLeniency: 0.7");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[Metadata]");
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("CircleSize:4");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine("0,279.06976744186,4,1,9,90,1,0");
            writer.WriteLine("[HitObjects]");

            for (int i = 0; i < 10; i++)
            {
                writer.WriteLine($"255,184,{i},69,4,3:1:0:0:");
            }

            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderBuilder(stream).Build();

            var file = fileReader.ReadFile();

            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects.Cast<StdHitObject>().ToList());

            //Act
            while (reader.ReadNext()) { }

            //Assert
            Assert.AreEqual(reader.GetHitObjectFromOffsetOrNull(offset), reader.GetHistoryEntryOrNull(offset)?.hitObject, "Expected to get the same HitObject");
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(1)]
        public void GetHistoryEntry_HitObjects_LogsHistory(int count)
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
            writer.WriteLine("CircleSize:4");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine("0,279.06976744186,4,1,9,90,1,0");
            writer.WriteLine("[HitObjects]");

            for (int i = 0; i < count; i++)
            {
                writer.WriteLine($"255,184,{i},69,4,3:1:0:0:");
            }

            writer.Flush();
            stream.Position = 0;

            var fileReader = new OsuFileReaderBuilder(stream).Build();

            var file = fileReader.ReadFile();

            var reader = new StdHitObjectReader(file.Difficulty, file.TimingPoints, file.HitObjects.Cast<StdHitObject>().ToList());

            //Act
            while (reader.ReadNext()) { }

            //Assert
            var actualHistory = new List<IHitObject>();

            for (int i = 0; i < count; i++)
            {
                actualHistory.Add(reader.GetHistoryEntryOrNull(i * - 1)?.hitObject);
            }

            actualHistory = actualHistory
                .OrderBy(item => item.TimeInMs)
                .ToList();

            CollectionAssert.AreEqual(actualHistory, file.HitObjects.ToList(), "Expected to be the same and log history correctly");
        }
    }
}
