using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.Enums;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using OsuFileIO.OsuFileReader.Exceptions;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileReader
{
    [TestClass]
    public class OsuFileReaderTests
    {
        private const string tutorialFile = "new beginnings.osu";
        private const string fileLocation = "OsuFileReader/TestFiles/";

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadGeneral_OsuFile_ReturnsGeneral()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var actual = reader.ReadGeneral();

            //Assert
            var expected = new General
            {
                Mode = GameMode.Standard,
                OsuFileFormat = 14,
                StackLeniency = 0.7,
            };

            Assert.AreEqual(expected.Mode, actual.Mode, $"Expected the file reader to read '{nameof(expected.Mode)}' correctly");
            Assert.AreEqual(expected.OsuFileFormat, actual.OsuFileFormat, $"Expected the file reader to read '{nameof(expected.OsuFileFormat)}' correctly");
            Assert.AreEqual(expected.StackLeniency, actual.StackLeniency, $"Expected the file reader to read '{nameof(expected.StackLeniency)}' correctly");
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadMetadata_OsuFile_ReturnsMetadata()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var actual = reader.ReadMetadata();

            //Assert
            var expected = new MetaData
            {
                Title = "new beginnings",
                TitleUnicode = "new beginnings",
                Artist = "nekodex",
                ArtistUnicode = "nekodex",
                Creator = "pishifat",
                Version = "tutorial",
                Source = "",
                Tags = "",
                BeatmapID = 2116202,
                BeatmapSetID = 1011011,
            };

            Assert.AreEqual(expected.Title, actual.Title, $"Expected the file reader to read '{nameof(expected.Title)}' correctly");
            Assert.AreEqual(expected.TitleUnicode, actual.TitleUnicode, $"Expected the file reader to read '{nameof(expected.TitleUnicode)}' correctly");
            Assert.AreEqual(expected.Artist, actual.Artist, $"Expected the file reader to read '{nameof(expected.Artist)}' correctly");
            Assert.AreEqual(expected.ArtistUnicode, actual.ArtistUnicode, $"Expected the file reader to read '{nameof(expected.ArtistUnicode)}' correctly");
            Assert.AreEqual(expected.Creator, actual.Creator, $"Expected the file reader to read '{nameof(expected.Creator)}' correctly");
            Assert.AreEqual(expected.Version, actual.Version, $"Expected the file reader to read '{nameof(expected.Version)}' correctly");
            Assert.AreEqual(expected.Source, actual.Source, $"Expected the file reader to read '{nameof(expected.Source)}' correctly");
            Assert.AreEqual(expected.Tags, actual.Tags, $"Expected the file reader to read '{nameof(expected.Tags)}' correctly");
            Assert.AreEqual(expected.BeatmapID, actual.BeatmapID, $"Expected the file reader to read '{nameof(expected.BeatmapID)}' correctly");
            Assert.AreEqual(expected.BeatmapSetID, actual.BeatmapSetID, $"Expected the file reader to read '{nameof(expected.BeatmapSetID)}' correctly");
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadDifficulty_OsuFile_ReturnsMetadataDifficulty()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var actual = reader.ReadDifficulty();

            //Assert
            var expected = new Difficulty
            {
                HPDrainRate = 0,
                CircleSize = 2,
                OverallDifficulty = 0,
                ApproachRate = 2,
                SliderMultiplier = 0.7,
                SliderTickRate = 1,
            };

            Assert.AreEqual(expected.HPDrainRate, actual.HPDrainRate, $"Expected the file reader to read '{nameof(expected.HPDrainRate)}' correctly");
            Assert.AreEqual(expected.CircleSize, actual.CircleSize, $"Expected the file reader to read '{nameof(expected.CircleSize)}' correctly");
            Assert.AreEqual(expected.OverallDifficulty, actual.OverallDifficulty, $"Expected the file reader to read '{nameof(expected.OverallDifficulty)}' correctly");
            Assert.AreEqual(expected.ApproachRate, actual.ApproachRate, $"Expected the file reader to read '{nameof(expected.ApproachRate)}' correctly");
            Assert.AreEqual(expected.SliderMultiplier, actual.SliderMultiplier, $"Expected the file reader to read '{nameof(expected.SliderMultiplier)}' correctly");
            Assert.AreEqual(expected.SliderTickRate, actual.SliderTickRate, $"Expected the file reader to read '{nameof(expected.SliderTickRate)}' correctly");
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadTimingPoints_OsuFile_ReturnsTimingPoints()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var actual = reader.ReadTimingPoints();

            //Assert
            var expectedCount = 17;
            Assert.IsTrue(actual.Count == expectedCount, $"Expected a list with {expectedCount} TimingPoints");
        }


        [TestMethod]
        [DataRow("-28,461.538461538462,4,1,0,100,1,0")]
        [DataRow("34125,-100,4,1,0,87,0,0")]
        public void ReadTimingPoints_TimingPointData_ReturnsTimingPoint(string timingPoint)
        {
            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("[General]");
            writer.WriteLine("Mode: 0");
            writer.WriteLine("[TimingPoints]");
            writer.WriteLine(timingPoint);
            writer.Flush();
            stream.Position = 0;

            var reader = new OsuFileReaderFactory(stream).Build();

            //Act
            var actual = reader.ReadTimingPoints().Single();

            //Assert
            var expected = timingPoint.Split(',');
            Assert.AreEqual(expected[0], actual.TimeInMs.ToString(), $"Expected the file reader to read '{nameof(actual.TimeInMs)}' correctly");
            Assert.AreEqual(expected[1], actual.BeatLength.ToString(), $"Expected the file reader to read '{nameof(actual.BeatLength)}' correctly");
            Assert.AreEqual(expected[2], actual.Meter.ToString(), $"Expected the file reader to read '{nameof(actual.Meter)}' correctly");
        }


        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadGeneral_OsuFileReadWrongOrder_ThrowsException()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            _ = reader.ReadTimingPoints();
            void actual() => reader.ReadGeneral();

            Assert.ThrowsException<OsuFileReaderException>(actual);
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadMetadata_OsuFileReadWrongOrder_ThrowsException()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            _ = reader.ReadTimingPoints();
            void actual() => reader.ReadMetadata();

            Assert.ThrowsException<OsuFileReaderException>(actual);
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadDifficulty_OsuFileReadWrongOrder_ThrowsException()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            _ = reader.ReadTimingPoints();
            void actual() => reader.ReadDifficulty();

            Assert.ThrowsException<OsuFileReaderException>(actual);
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadTimingPoints_OsuFileReadTwice_ThrowsException()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            _ = reader.ReadTimingPoints();
            void actual() => reader.ReadTimingPoints();

            Assert.ThrowsException<OsuFileReaderException>(actual);
        }

        [TestMethod]
        [DeploymentItem(fileLocation + tutorialFile)]
        public void ReadGeneral_OsuFileReadTwiceWithReset_ReturnsGeneral()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var read1 = reader.ReadGeneral();

            reader.ResetReader();

            var read2 = reader.ReadGeneral();

            //Arrange
            Assert.AreEqual(read1.Mode, read2.Mode, $"Expected the re-read to be the same");
            Assert.AreEqual(read1.OsuFileFormat, read2.OsuFileFormat, $"Expected the re-read to be the same");
            Assert.AreEqual(read1.StackLeniency, read2.StackLeniency, $"Expected the re-read to be the same");
        }
    }
}
