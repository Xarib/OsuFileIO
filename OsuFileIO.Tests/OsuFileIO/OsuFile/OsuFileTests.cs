using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.OsuFile;

[TestClass]
public class OsuFileTests
{
    [TestMethod]
    public void Equal_EqualGeneralObjects_ReturnsTrue()
    {
        General lhs = null;
        General rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new General()
        {
            Mode = Enums.GameMode.Standard,
            OsuFileFormat = 14,
            StackLeniency = 0.6,
        };

        rhs = new General()
        {
            Mode = Enums.GameMode.Standard,
            OsuFileFormat = 14,
            StackLeniency = 0.6,
        };

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalGeneralObjects_ReturnsTrue()
    {
        General lhs = null;
        General rhs = new General()
        {
            Mode = Enums.GameMode.Standard,
            OsuFileFormat = 14,
            StackLeniency = 0.6,
        }; ;
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        rhs = null;
        lhs = new General()
        {
            Mode = Enums.GameMode.Standard,
            OsuFileFormat = 14,
            StackLeniency = 0.6,
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        rhs = new General()
        {
            Mode = Enums.GameMode.Standard,
            OsuFileFormat = 10000,
            StackLeniency = 0.6,
        };

        Assert.IsTrue(lhs != rhs, "Expected to be unequal");
    }

    [TestMethod]
    public void Equal_EqualMetadataObjects_ReturnsTrue()
    {
        MetaData lhs = null;
        MetaData rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new MetaData()
        {
            Artist = "1",
            ArtistUnicode = "2",
            BeatmapID = 3,
            BeatmapSetID = 4,
            Creator = "5",
            Source = "6",
            Tags = "7",
            Title = "8",
            TitleUnicode = "9",
            Version = "10",
        };

        rhs = new MetaData()
        {
            Artist = "1",
            ArtistUnicode = "2",
            BeatmapID = 3,
            BeatmapSetID = 4,
            Creator = "5",
            Source = "6",
            Tags = "7",
            Title = "8",
            TitleUnicode = "9",
            Version = "10",
        };

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalMetadataObjects_ReturnsTrue()
    {
        MetaData lhs = null;
        MetaData rhs = new MetaData()
        {
            Artist = "1",
            ArtistUnicode = "2",
            BeatmapID = 3,
            BeatmapSetID = 4,
            Creator = "5",
            Source = "6",
            Tags = "7",
            Title = "8",
            TitleUnicode = "9",
            Version = "10",
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        lhs = new MetaData()
        {
            Artist = "1",
            ArtistUnicode = "2",
            BeatmapID = 3,
            BeatmapSetID = 4,
            Creator = "5",
            Source = "6",
            Tags = "7",
            Title = "8",
            TitleUnicode = "9",
            Version = "10",
        };
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        rhs = new MetaData()
        {
            Artist = "1",
            ArtistUnicode = "2",
            BeatmapID = 3,
            BeatmapSetID = 4,
            Creator = "33333333333333335",
            Source = "6",
            Tags = "7",
            Title = "8",
            TitleUnicode = "9",
            Version = "10",
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");
    }

    [TestMethod]
    public void Equal_EqualDifficultyObjects_ReturnsTrue()
    {
        Difficulty lhs = null;
        Difficulty rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new Difficulty
        {
            ApproachRate = 1,
            CircleSize = 2,
            HPDrainRate = 3,
            OverallDifficulty = 4,
            SliderMultiplier = 5,
            SliderTickRate = 6,
        };

        rhs = new Difficulty
        {
            ApproachRate = 1,
            CircleSize = 2,
            HPDrainRate = 3,
            OverallDifficulty = 4,
            SliderMultiplier = 5,
            SliderTickRate = 6,
        };

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalDifficultyObjects_ReturnsTrue()
    {
        Difficulty lhs = null;
        Difficulty rhs = new Difficulty
        {
            ApproachRate = 1,
            CircleSize = 2,
            HPDrainRate = 3,
            OverallDifficulty = 4,
            SliderMultiplier = 5,
            SliderTickRate = 6,
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        lhs = new Difficulty
        {
            ApproachRate = 1,
            CircleSize = 2,
            HPDrainRate = 3,
            OverallDifficulty = 4,
            SliderMultiplier = 5,
            SliderTickRate = 6,
        };
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        rhs = new Difficulty
        {
            ApproachRate = 1,
            CircleSize = 2,
            HPDrainRate = 3,
            OverallDifficulty = 1999999,
            SliderMultiplier = 5,
            SliderTickRate = 6,
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");
    }

    [TestMethod]
    public void Equal_EqualTimingPointObjects_ReturnsTrue()
    {
        TimingPoint lhs = null;
        TimingPoint rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new TimingPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
        };

        rhs = new TimingPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
        };

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }

    [TestMethod]
    public void NotEqual_UnequalTimingPointObjects_ReturnsTrue()
    {
        TimingPoint lhs = null;
        TimingPoint rhs = new TimingPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        lhs = new TimingPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
        };
        rhs = null;
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");

        rhs = new TimingPoint()
        {
            BeatLength = 1,
            Meter = 44442,
            TimeInMs = 3,
        };
        Assert.IsTrue(lhs != rhs, "Expected to be unequal");
    }

    [TestMethod]
    public void Equal_EqualInheritedPointObjects_ReturnsTrue()
    {
        InheritedPoint lhs = null;
        InheritedPoint rhs = null;
        Assert.IsTrue(lhs == rhs, "Expected to be equal");

        lhs = new InheritedPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
            VelocityMultiplier = 4,
        };

        rhs = new InheritedPoint()
        {
            BeatLength = 1,
            Meter = 2,
            TimeInMs = 3,
            VelocityMultiplier = 4,
        };

        Assert.IsTrue(lhs == rhs, "Expected to be equal");
    }
}
