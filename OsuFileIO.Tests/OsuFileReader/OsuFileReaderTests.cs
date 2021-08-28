﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.Enums;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
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
        public void ReadTimingPoints_OsuFile_ReturnsReadTimingPoints()
        {
            //Arrange
            var reader = new OsuFileReaderFactory(tutorialFile).Build();

            //Act
            var actual = reader.ReadTimingPoints();

            //Assert
            var expectedCount = 17;
            Assert.IsTrue(actual.Count == expectedCount, $"Expected a list with {expectedCount} TimingPoints");
        }
    }
}
