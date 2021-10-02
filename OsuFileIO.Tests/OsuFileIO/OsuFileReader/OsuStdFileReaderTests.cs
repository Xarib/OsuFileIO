using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.OsuFileReader
{
    [TestClass]
    public class OsuStdFileReaderTests
    {
        private const string fileLocation = "TestFiles/";
        private const string problematic = "TestFiles/Problematic/";

        [TestMethod]
        [DataRow("364,180,2185,1,2,0:0:0:0:")]
        [DataRow("140,180,5821,1,2,0:0:0:0:")]
        public void ReadFile_HitObjectData_ReturnsCircle(string line)
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
            writer.WriteLine("[HitObjects]");
            writer.WriteLine(line);
            writer.Flush();
            stream.Position = 0;

            var reader = new OsuFileReaderFactory(stream).Build();

            //Act
            var file = reader.ReadFile();
            var actual = file.HitObjects.Single() as Circle;
            //Assert
            var expected = line.Split(',');
            Assert.IsTrue(actual is Circle, $"Expected a {nameof(Circle)}");
            Assert.AreEqual(new Coordinates(int.Parse(expected[0]), int.Parse(expected[1])), actual.Coordinates, "Expected to read coordiantes correctly");
            Assert.AreEqual(int.Parse(expected[2]), actual.TimeInMs, "Expected to get the correct time");
            Assert.AreEqual(actual.Coordinates, actual.EndCoordinates, "Must be same");
        }

        [TestMethod]
        [DataRow("256,192,126433,12,4,129202,3:2:0:0:")]
        [DataRow("256,192,128207,12,8,130025,0:0:0:0:")]
        public void ReadFile_HitObjectData_ReturnsSpinner(string line)
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
            writer.WriteLine("[HitObjects]");
            writer.WriteLine(line);
            writer.Flush();
            stream.Position = 0;

            var reader = new OsuFileReaderFactory(stream).Build();

            //Act
            var file = reader.ReadFile();
            var actual = file.HitObjects.Single() as Spinner;

            //Assert
            var expected = line.Split(',');
            Assert.IsTrue(actual is Spinner, $"Expected a {nameof(Spinner)}");
            Assert.AreEqual(new Coordinates(int.Parse(expected[0]), int.Parse(expected[1])), actual.Coordinates, "Expected to read coordiantes correctly");
            Assert.AreEqual(int.Parse(expected[2]), actual.TimeInMs, "Expected to get the correct time");
            Assert.AreEqual(int.Parse(expected[5]), actual.EndTimeInMs, "Expected to get the correct end time");
            Assert.AreEqual(actual.Coordinates, actual.EndCoordinates, "Must be same");
        }

        [TestMethod]
        [DataRow("80,136,133207,2,0,B|114:147|114:147|164:128|164:128|240:144,11,142.5,10|0,0:0|0:0,0:0:0:0:")]
        [DataRow("235,22,173662,6,0,L|295:146,8,113.999996520996,14|2,0:0|0:0,0:0:0:0:")]
        public void ReadFile_HitObjectData_ReturnsSlider(string line)
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
            writer.WriteLine("[HitObjects]");
            writer.WriteLine(line);
            writer.Flush();
            stream.Position = 0;

            var reader = new OsuFileReaderFactory(stream).Build();

            //Act
            var file = reader.ReadFile();
            var actual = file.HitObjects.Single() as Slider;

            //Assert
            var expected = line.Split(',');
            var hitObjectLine = expected[5];
            var curveType = (CurveType)hitObjectLine[0];
            var points = hitObjectLine
                .Split('|')
                .Skip(1)
                .Distinct()
                .Select(str => new Coordinates(int.Parse(str[0..str.IndexOf(':')]), int.Parse(str[(str.IndexOf(':') + 1)..])))
                .ToList();
            var slides = int.Parse(expected[6]);
            var length = double.Parse(expected[7]);
            var coords = new Coordinates(int.Parse(expected[0]), int.Parse(expected[1]));
            var expectedCoords = slides % 2 == 1 ? coords : points.Last();

            Assert.IsTrue(actual is Slider, $"Expected a {nameof(Slider)}");
            Assert.AreEqual(coords, actual.Coordinates, "Expected to read coordiantes correctly");
            Assert.AreEqual(int.Parse(expected[2]), actual.TimeInMs, "Expected to get the correct time");
            CollectionAssert.AreEqual(points, actual.SliderCoordinates, "Expected to get the correct time");
            Assert.AreEqual(length, actual.Length, "Expected to get the correct slider length");
            Assert.AreEqual(curveType, actual.CurveType, "Expected to read the correct curve type");
            Assert.AreEqual(slides, actual.Slides, "Expected to read slides correctly");
            Assert.AreEqual(length * slides, actual.TravelLenth, "Expected to calculate travel length");
            Assert.AreEqual(expectedCoords, actual.EndCoordinates, "Expected to get the correct end coords");
        }

        [TestMethod]
        [DeploymentItem(fileLocation + "stdShort.osu")]
        [DeploymentItem(fileLocation + "stdShortNoNewLines.osu")]
        public void ReadFile_OsuFileWithoutNewLinesBetween_ReturnsSameHitObjectsWithNewLines()
        {
            //Arrange
            var reader1 = new OsuFileReaderFactory("stdShort.osu").Build();
            var reader2 = new OsuFileReaderFactory("stdShortNoNewLines.osu").Build();

            //Act
            var actual1 = reader1.ReadFile();
            var actual2 = reader2.ReadFile();

            //Assert
            CollectionAssert.AreEqual(actual1.HitObjects, actual2.HitObjects, $"Expected to get the same HitObjects");
        }

        [TestMethod]
        [DeploymentItem(problematic + "100.osu")]
        [DeploymentItem(problematic + "1000168.osu")]
        [DataRow("100.osu")]
        [DataRow("1000168.osu")]
        public void ReadFile_ProblematicFile100_ReturnsOsuFile(string fileName)
        {
            //Arrange
            var reader = new OsuFileReaderFactory(fileName).Build();

            //Act
            var actual = reader.ReadFile();
            //Should not throw
        }

    }
}
