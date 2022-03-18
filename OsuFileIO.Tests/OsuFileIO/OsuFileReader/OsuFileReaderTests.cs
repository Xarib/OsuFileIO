using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.Enums;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using OsuFileIO.OsuFileReader.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.OsuFileReader;

[TestClass]
public class OsuFileReaderTests
{
    private const string tutorialFile = "new beginnings.osu";
    private const string catchFile = "catch.osu";
    private const string maniaFile = "mania.osu";
    private const string taikoFile = "taiko.osu";
    private const string fileLocation = "TestFiles/";

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    [DeploymentItem(fileLocation + catchFile)]
    [DeploymentItem(fileLocation + maniaFile)]
    [DeploymentItem(fileLocation + taikoFile)]
    [DataRow(tutorialFile, GameMode.Standard, 14, 0.7)]
    [DataRow(catchFile, GameMode.Catch, 14, 0.7)]
    [DataRow(maniaFile, GameMode.Mania, 14, 0.7)]
    [DataRow(taikoFile, GameMode.Taiko, 14, 0.5)]
    public void ReadGeneral_OsuFile_ReturnsGeneral(string fileName, GameMode gameMode, int format, double leniency)
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(fileName).Build();

        //Act
        var actual = reader.ReadGeneral();

        //Assert
        var expected = new General
        {
            Mode = gameMode,
            OsuFileFormat = format,
            StackLeniency = leniency,
        };

        Assert.AreEqual(expected.Mode, actual.Mode, $"Expected the file reader to read '{nameof(expected.Mode)}' correctly");
        Assert.AreEqual(expected.OsuFileFormat, actual.OsuFileFormat, $"Expected the file reader to read '{nameof(expected.OsuFileFormat)}' correctly");
        Assert.AreEqual(expected.StackLeniency, actual.StackLeniency, $"Expected the file reader to read '{nameof(expected.StackLeniency)}' correctly");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    [DeploymentItem(fileLocation + catchFile)]
    [DeploymentItem(fileLocation + maniaFile)]
    [DeploymentItem(fileLocation + taikoFile)]
    [DataRow(tutorialFile, "new beginnings", "new beginnings", "nekodex", "nekodex", "pishifat", "tutorial", "", new string[0] { }, 2116202, 1011011)]
    [DataRow(taikoFile, "Amatsu Kitsune", "アマツキツネ", "YURiCa", "ユリカ", "Kyuukai", "Charlotte's Inner Oni", "初音ミク Project mirai 2", new string[] { "marasy", "Hanatan", "花たん" }, 1521524, 716642)]
    [DataRow(catchFile, "Fukagyaku Replace", "不可逆リプレイス", "MY FIRST STORY", "MY FIRST STORY", "Akitoshi", "Chara's Overdose", "信長協奏曲", new string[] { "Nobunaga", "Concerto", "Ending", "TV", "Size", "Asagi", "Ster", "koliron", "Irreversible", "Chara" }, 767324, 342218)]
    [DataRow(maniaFile, "Miracle 5ympho X", "Miracle 5ympho X", "USAO", "USAO", "Mel", "Fullerene's 4K Black Another", "beatmania IIDX 21 SPADA", new string[] { "frey", "sionkotori", "kokodoko", "ナウい", "一年一片", "frenchcore", "dubstep", "intensity", "fullerene", "kurokami", "gezo" }, 482546, 137394)]
    public void ReadMetadata_OsuFile_ReturnsMetadata(string fileName, string title, string titleUnicode, string artist, string artistUnicode, string creator, string version, string source, string[] tags, int beatmapID, int beatmapSetID)
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(fileName).Build();

        //Act
        var actual = reader.ReadMetadata();

        //Assert
        var expected = new Metadata
        {
            Title = title,
            TitleUnicode = titleUnicode,
            Artist = artist,
            ArtistUnicode = artistUnicode,
            Creator = creator,
            Version = version,
            Source = source,
            Tags = tags,
            BeatmapID = beatmapID,
            BeatmapSetID = beatmapSetID,
        };

        Assert.AreEqual(expected.Title, actual.Title, $"Expected the file reader to read '{nameof(expected.Title)}' correctly");
        Assert.AreEqual(expected.TitleUnicode, actual.TitleUnicode, $"Expected the file reader to read '{nameof(expected.TitleUnicode)}' correctly");
        Assert.AreEqual(expected.Artist, actual.Artist, $"Expected the file reader to read '{nameof(expected.Artist)}' correctly");
        Assert.AreEqual(expected.ArtistUnicode, actual.ArtistUnicode, $"Expected the file reader to read '{nameof(expected.ArtistUnicode)}' correctly");
        Assert.AreEqual(expected.Creator, actual.Creator, $"Expected the file reader to read '{nameof(expected.Creator)}' correctly");
        Assert.AreEqual(expected.Version, actual.Version, $"Expected the file reader to read '{nameof(expected.Version)}' correctly");
        Assert.AreEqual(expected.Source, actual.Source, $"Expected the file reader to read '{nameof(expected.Source)}' correctly");
        CollectionAssert.AreEqual(expected.Tags, actual.Tags, $"Expected the file reader to read '{nameof(expected.Tags)}' correctly");
        Assert.AreEqual(expected.BeatmapID, actual.BeatmapID, $"Expected the file reader to read '{nameof(expected.BeatmapID)}' correctly");
        Assert.AreEqual(expected.BeatmapSetID, actual.BeatmapSetID, $"Expected the file reader to read '{nameof(expected.BeatmapSetID)}' correctly");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    [DeploymentItem(fileLocation + catchFile)]
    [DeploymentItem(fileLocation + maniaFile)]
    [DeploymentItem(fileLocation + taikoFile)]
    [DataRow(tutorialFile, 0, 2, 0, 2, 0.7, 1)]
    [DataRow(catchFile, 7, 4, 9.3, 9.3, 2.4, 2)]
    [DataRow(maniaFile, 8, 4, 8, 5, 1.4, 1)]
    [DataRow(taikoFile, 6, 5, 6, 4.5, 1.4, 1)]
    public void ReadDifficulty_OsuFile_ReturnsMetadataDifficulty(string fileName, double HP, double CS, double OD, double AR, double SM, double STR)
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(fileName).Build();

        //Act
        var actual = reader.ReadDifficulty();

        //Assert
        var expected = new Difficulty
        {
            HPDrainRate = HP,
            CircleSize = CS,
            OverallDifficulty = OD,
            ApproachRate = AR,
            SliderMultiplier = SM,
            SliderTickRate = STR,
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
    [DeploymentItem(fileLocation + catchFile)]
    [DeploymentItem(fileLocation + maniaFile)]
    [DeploymentItem(fileLocation + taikoFile)]
    [DataRow(tutorialFile, 17)]
    [DataRow(catchFile, 133)]
    [DataRow(maniaFile, 8)]
    [DataRow(taikoFile, 15)]
    public void ReadTimingPoints_OsuFile_ReturnsTimingPoints(string fileName, int count)
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(fileName).Build();

        //Act
        var actual = reader.ReadTimingPoints();

        //Assert
        Assert.AreEqual(actual.Count, count, $"Expected a list with {actual.Count} TimingPoints");
    }


    [TestMethod]
    [DataRow("-28,461.538461538462,4,1,0,100,1,0")]
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

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        var actual = reader.ReadTimingPoints().Single();

        //Assert
        var expected = timingPoint.Split(',');
        Assert.IsTrue(actual is not InheritedPoint, $"Expected a {nameof(TimingPoint)} to be created");
        Assert.AreEqual(expected[0], actual.TimeInMs.ToString(), $"Expected the file reader to read '{nameof(actual.TimeInMs)}' correctly");
        Assert.AreEqual(expected[1], actual.BeatLength.ToString(), $"Expected the file reader to read '{nameof(actual.BeatLength)}' correctly");
        Assert.AreEqual(expected[2], actual.Meter.ToString(), $"Expected the file reader to read '{nameof(actual.Meter)}' correctly");
    }

    [TestMethod]
    [DataRow("34125,-100,4,1,0,87,0,0")]
    [DataRow("36986,-83.3333333333333,4,1,1,70,0,0")]
    public void ReadTimingPoints_InheritedPointData_ReturnsTimingPoint(string timingPoint)
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("-28,461.538461538462,4,1,0,100,1,0");
        writer.WriteLine(timingPoint);
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        var timingPoints = reader.ReadTimingPoints().ToList();
        var expectedTimingPoint = timingPoints[0];
        var actual = timingPoints.Last() as InheritedPoint;

        //Assert
        var expectedInheritedPoint = timingPoint.Split(',');
        Assert.IsTrue(actual is InheritedPoint, $"Expected a {nameof(InheritedPoint)} to be created");
        Assert.AreEqual(expectedInheritedPoint[0], actual.TimeInMs.ToString(), $"Expected the file reader to read '{nameof(actual.TimeInMs)}' correctly");
        Assert.AreEqual(expectedTimingPoint.BeatLength, actual.BeatLength, $"Expected the file reader to read '{nameof(actual.BeatLength)}' correctly");
        Assert.AreEqual(expectedTimingPoint.Meter, actual.Meter, $"Expected the file reader to read '{nameof(actual.Meter)}' correctly");
        Assert.AreEqual(-100d / double.Parse(expectedInheritedPoint[1]), actual.VelocityMultiplier, $"Expected the file reader to calculate '{nameof(actual.VelocityMultiplier)}' correctly");
    }

    [TestMethod]
    public void ReadTimingPoints_InheritedPointWithNoTimingPoint_ReturnsTimingPoints()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("34125,-100,4,1,0,87,0,0");
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        reader.ReadTimingPoints();
    }

    [TestMethod]
    public void ReadTimingPoints_InheritedPointWithNoTimingPointWithStrict_ThrowsOsuFileReaderException()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("34125,-100,4,1,0,87,0,0");
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream)
            .UseOptions(new OsuFileReaderOptions
            {
                StrictTimingPointInheritance = true,
            })
            .Build();

        //Act
        void actual() => reader.ReadTimingPoints().Single();

        //Assert
        Assert.ThrowsException<OsuFileReaderException>(actual);
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    public void ReadGeneral_OsuFileReadWrongOrder_ThrowsException()
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(tutorialFile).Build();

        //Act
        _ = reader.ReadTimingPoints();
        void actual() => reader.ReadGeneral();

        Assert.ThrowsException<OsuFileReaderException>(actual);
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    public void ReadGeneral_OsuFileReadTwiceWithReset_ReturnsGeneral()
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(tutorialFile).Build();

        //Act
        var read1 = reader.ReadGeneral();

        reader.ResetReader();

        var read2 = reader.ReadGeneral();

        //Arrange
        Assert.AreEqual(read1.Mode, read2.Mode, $"Expected the re-read to be the same");
        Assert.AreEqual(read1.OsuFileFormat, read2.OsuFileFormat, $"Expected the re-read to be the same");
        Assert.AreEqual(read1.StackLeniency, read2.StackLeniency, $"Expected the re-read to be the same");
    }

    [TestMethod]
    public void ReadGeneral_NoModeInFile_ReturnsGeneralWithGamemodeStandard()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("StackLeniency: 0.7");
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        var general = reader.ReadGeneral();

        //Arrange
        Assert.AreEqual(GameMode.Standard, general.Mode, $"Expected {GameMode.Standard} because there was no mode given");
    }

    [TestMethod]
    public void ReadGeneral_WithSpaceForFormat_ReturnsGeneralWithGamemodeStandard()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("     osu file format v14");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        var general = reader.ReadGeneral();

        //Arrange
        Assert.AreEqual(14, general.OsuFileFormat, $"Expected {14} because there was no mode given");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    public void ReadMetadata_OsuFileReadWrongOrder_ThrowsException()
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(tutorialFile).Build();

        //Act
        _ = reader.ReadTimingPoints();
        void actual() => reader.ReadMetadata();

        Assert.ThrowsException<OsuFileReaderException>(actual);
    }

    [TestMethod]
    public void ReadMetadata_MetadataRandomNewLine()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[Metadata]");
        writer.WriteLine("Title:dddd");
        writer.WriteLine("Tags:test");
        writer.WriteLine("randomnewline");
        writer.WriteLine("Creator:727");
        writer.Flush();
        stream.Position = 0;

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        var actual = reader.ReadMetadata();

        //Assert
        Assert.AreEqual("dddd", actual.Title, $"Expected the file reader to read '{nameof(actual.Title)}' correctly");
        CollectionAssert.AreEqual(new string[] { "test" }, actual.Tags, $"Expected the file reader to read '{nameof(actual.Tags)}' correctly");
        Assert.AreEqual("727", actual.Creator, $"Expected the file reader to read '{nameof(actual.Creator)}' correctly");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    public void ReadDifficulty_OsuFileReadWrongOrder_ThrowsException()
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(tutorialFile).Build();

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
        var reader = new OsuFileReaderBuilder(tutorialFile).Build();

        //Act
        _ = reader.ReadTimingPoints();
        void actual() => reader.ReadTimingPoints();

        //Assert
        Assert.ThrowsException<OsuFileReaderException>(actual);
    }

    [TestMethod]
    [DeploymentItem(fileLocation + tutorialFile)]
    public void DisposeReader_WhenReaderIsBuilt_ThrowsObjectDisposedException()
    {
        //Arrange
        var stream = File.OpenRead(tutorialFile);

        var reader = new OsuFileReaderBuilder(stream).Build();

        //Act
        reader.Dispose();
        void actual() => stream.ReadByte();

        //Assert
        Assert.ThrowsException<ObjectDisposedException>(actual);
    }

    [TestMethod]
    [DeploymentItem(fileLocation + "stdShort.osu")]
    [DeploymentItem(fileLocation + "stdShortNoNewLines.osu")]
    public void Read_OsuFileWithoutNewLinesBetween_ReturnsSameResultWithNewLines()
    {
        //Arrange
        var reader1 = new OsuFileReaderBuilder("stdShort.osu").Build();
        var reader2 = new OsuFileReaderBuilder("stdShortNoNewLines.osu").Build();

        //Act
        var actual1 = reader1.ReadFile();
        var actual2 = reader2.ReadFile();

        //Assert
        Assert.AreEqual(actual1.General, actual2.General, $"Expected to get the same {nameof(General)}");
        Assert.AreEqual(actual1.MetaData, actual2.MetaData, $"Expected to get the same {nameof(Metadata)}");
        Assert.AreEqual(actual1.Difficulty, actual2.Difficulty, $"Expected to get the same {nameof(Difficulty)}");
        CollectionAssert.AreEqual(actual1.TimingPoints, actual2.TimingPoints, $"Expected to get the same {nameof(TimingPoint)}s");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + "new beginnings utf8 bom.osu")]
    public void ReadFile_OsuFileUTF8BOM_ShouldReset()
    {
        //Arrange
        var reader = new OsuFileReaderBuilder("new beginnings utf8 bom.osu").Build();

        //Act
        var read1 = reader.ReadGeneral();

        reader.ResetReader();

        var read2 = reader.ReadGeneral();

        //Assert
        Assert.AreEqual(read1.Mode, read2.Mode, $"Expected the re-read to be the same");
        Assert.AreEqual(read1.OsuFileFormat, read2.OsuFileFormat, $"Expected the re-read to be the same");
        Assert.AreEqual(read1.StackLeniency, read2.StackLeniency, $"Expected the re-read to be the same");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + "empty.osu")]
    public void ReadFile_EmptyFile_ShouldThrowArgumentException()
    {
        //Act
        void actual() => new OsuFileReaderBuilder("empty.osu");

        //Assert
        Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void ReadFile_MetadataOverridesNoFileMetadata_ShouldOverride()
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
        writer.WriteLine("SliderMultiplier: 0.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("562,574.162679425837,4,2,0,30,1,0");
        writer.Flush();
        stream.Position = 0;

        //Act
        var overrides = new OsuFileReaderOverride
        {
            MetaData = new Metadata
            {
                Artist = "1",
                ArtistUnicode = "2",
                BeatmapID = 3,
                BeatmapSetID = 4,
                Creator = "5",
                Source = "6",
                Tags = new string[] { "7" },
                Title = "8",
                TitleUnicode = "9",
                Version = "10",
            },
        };

        var reader = new OsuFileReaderBuilder(stream)
            .UseOverrides(overrides)
            .Build();

        var beatmap = reader.ReadFile();

        //Assert
        var expected = overrides.MetaData;
        var actual = beatmap.MetaData;
        Assert.AreEqual(expected.Artist, actual.Artist, $"Expected to override {nameof(actual.Artist)}");
        Assert.AreEqual(expected.ArtistUnicode, actual.ArtistUnicode, $"Expected to override {nameof(actual.ArtistUnicode)}");
        Assert.AreEqual(expected.BeatmapID, actual.BeatmapID, $"Expected to override {nameof(actual.BeatmapID)}");
        Assert.AreEqual(expected.BeatmapSetID, actual.BeatmapSetID, $"Expected to override {nameof(actual.BeatmapSetID)}");
        Assert.AreEqual(expected.Creator, actual.Creator, $"Expected to override {nameof(actual.Creator)}");
        Assert.AreEqual(expected.Source, actual.Source, $"Expected to override {nameof(actual.Source)}");
        CollectionAssert.AreEqual(expected.Tags, actual.Tags, $"Expected to override {nameof(actual.Tags)}");
        Assert.AreEqual(expected.Title, actual.Title, $"Expected to override {nameof(actual.Title)}");
        Assert.AreEqual(expected.TitleUnicode, actual.TitleUnicode, $"Expected to override {nameof(actual.TitleUnicode)}");
        Assert.AreEqual(expected.Version, actual.Version, $"Expected to override {nameof(actual.Version)}");
    }

    [TestMethod]
    public void ReadFile_MetadataOverrides_ShouldOverride()
    {
        //Arrange
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[Metadata]");
        writer.WriteLine("Title:Goodbye");
        writer.WriteLine("TitleUnicode:Goodbye");
        writer.WriteLine("Artist:BLANKFIELD");
        writer.WriteLine("ArtistUnicode:BLANKFIELD");
        writer.WriteLine("Creator:Kyubey");
        writer.WriteLine("Version:Intense");
        writer.WriteLine("Source:Undertale");
        writer.WriteLine("Tags:asgore toby fox");
        writer.WriteLine("BeatmapID:1172819");
        writer.WriteLine("BeatmapSetID:553906");
        writer.WriteLine("[Difficulty]");
        writer.WriteLine("CircleSize:4");
        writer.WriteLine("SliderMultiplier: 0.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("562,574.162679425837,4,2,0,30,1,0");
        writer.Flush();
        stream.Position = 0;

        //Act
        var overrides = new OsuFileReaderOverride
        {
            MetaData = new Metadata
            {
                Artist = "1",
                ArtistUnicode = "2",
                BeatmapID = 3,
                BeatmapSetID = 4,
                Creator = "5",
                Source = "6",
                Tags = new string[] { "7" },
                Title = "8",
                TitleUnicode = "9",
                Version = "10",
            },
        };

        var reader = new OsuFileReaderBuilder(stream)
            .UseOverrides(overrides)
            .Build();

        var beatmap = reader.ReadFile();

        //Assert
        var expected = overrides.MetaData;
        var actual = beatmap.MetaData;
        Assert.AreEqual(expected.Artist, actual.Artist, $"Expected to override {nameof(actual.Artist)}");
        Assert.AreEqual(expected.ArtistUnicode, actual.ArtistUnicode, $"Expected to override {nameof(actual.ArtistUnicode)}");
        Assert.AreEqual(expected.BeatmapID, actual.BeatmapID, $"Expected to override {nameof(actual.BeatmapID)}");
        Assert.AreEqual(expected.BeatmapSetID, actual.BeatmapSetID, $"Expected to override {nameof(actual.BeatmapSetID)}");
        Assert.AreEqual(expected.Creator, actual.Creator, $"Expected to override {nameof(actual.Creator)}");
        Assert.AreEqual(expected.Source, actual.Source, $"Expected to override {nameof(actual.Source)}");
        CollectionAssert.AreEqual(expected.Tags, actual.Tags, $"Expected to override {nameof(actual.Tags)}");
        Assert.AreEqual(expected.Title, actual.Title, $"Expected to override {nameof(actual.Title)}");
        Assert.AreEqual(expected.TitleUnicode, actual.TitleUnicode, $"Expected to override {nameof(actual.TitleUnicode)}");
        Assert.AreEqual(expected.Version, actual.Version, $"Expected to override {nameof(actual.Version)}");
    }

    [TestMethod]
    public void ReadFile_DifficultyOverrides_ShouldOverride()
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
        writer.WriteLine("HPDrainRate:4.7");
        writer.WriteLine("CircleSize:4");
        writer.WriteLine("OverallDifficulty:10");
        writer.WriteLine("ApproachRate:10");
        writer.WriteLine("SliderMultiplier:2");
        writer.WriteLine("SliderTickRate:2");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("562,574.162679425837,4,2,0,30,1,0");
        writer.Flush();
        stream.Position = 0;

        //Act
        var overrides = new OsuFileReaderOverride
        {
            Difficulty = new Difficulty
            {
                ApproachRate = 1.1,
                CircleSize = 2.2,
                HPDrainRate = 3.3,
                OverallDifficulty = 4.4,
                SliderMultiplier = 5.5,
                SliderTickRate = 6.6,
            }
        };

        var reader = new OsuFileReaderBuilder(stream)
            .UseOverrides(overrides)
            .Build();

        var beatmap = reader.ReadFile();

        //Assert
        var expected = overrides.Difficulty;
        var actual = beatmap.Difficulty;
        Assert.AreEqual(expected.ApproachRate, actual.ApproachRate, $"Expected to override {nameof(actual.ApproachRate)}");
        Assert.AreEqual(expected.CircleSize, actual.CircleSize, $"Expected to override {nameof(actual.CircleSize)}");
        Assert.AreEqual(expected.HPDrainRate, actual.HPDrainRate, $"Expected to override {nameof(actual.HPDrainRate)}");
        Assert.AreEqual(expected.OverallDifficulty, actual.OverallDifficulty, $"Expected to override {nameof(actual.OverallDifficulty)}");
        Assert.AreEqual(expected.SliderMultiplier, actual.SliderMultiplier, $"Expected to override {nameof(actual.SliderMultiplier)}");
        Assert.AreEqual(expected.SliderTickRate, actual.SliderTickRate, $"Expected to override {nameof(actual.SliderTickRate)}");
    }

    [TestMethod]
    public void ReadFile_FileDifficultyNotSetDifficultyOverrides_ShouldOverride()
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
        writer.WriteLine("562,574.162679425837,4,2,0,30,1,0");
        writer.Flush();
        stream.Position = 0;

        //Act
        var overrides = new OsuFileReaderOverride
        {
            Difficulty = new Difficulty
            {
                ApproachRate = 1.1,
                CircleSize = 2.2,
                HPDrainRate = 3.3,
                OverallDifficulty = 4.4,
                SliderMultiplier = 5.5,
                SliderTickRate = 6.6,
            }
        };

        var reader = new OsuFileReaderBuilder(stream)
            .UseOverrides(overrides)
            .Build();

        var beatmap = reader.ReadFile();

        //Assert
        var expected = overrides.Difficulty;
        var actual = beatmap.Difficulty;
        Assert.AreEqual(expected.ApproachRate, actual.ApproachRate, $"Expected to override {nameof(actual.ApproachRate)}");
        Assert.AreEqual(expected.CircleSize, actual.CircleSize, $"Expected to override {nameof(actual.CircleSize)}");
        Assert.AreEqual(expected.HPDrainRate, actual.HPDrainRate, $"Expected to override {nameof(actual.HPDrainRate)}");
        Assert.AreEqual(expected.OverallDifficulty, actual.OverallDifficulty, $"Expected to override {nameof(actual.OverallDifficulty)}");
        Assert.AreEqual(expected.SliderMultiplier, actual.SliderMultiplier, $"Expected to override {nameof(actual.SliderMultiplier)}");
        Assert.AreEqual(expected.SliderTickRate, actual.SliderTickRate, $"Expected to override {nameof(actual.SliderTickRate)}");
    }

    [TestMethod]
    public void ReadFile_GeneralOverrides_ShouldOverride()
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
        writer.WriteLine("HPDrainRate:4.7");
        writer.WriteLine("CircleSize:4");
        writer.WriteLine("OverallDifficulty:10");
        writer.WriteLine("ApproachRate:10");
        writer.WriteLine("SliderMultiplier:2");
        writer.WriteLine("SliderTickRate:2");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine("1266,279.06976744186,4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("562,574.162679425837,4,2,0,30,1,0");
        writer.Flush();
        stream.Position = 0;

        //Act
        var overrides = new OsuFileReaderOverride
        {
            General = new General
            {
                Mode = GameMode.Catch,
                OsuFileFormat = 15,
                StackLeniency = 10
            }
        };

        var reader = new OsuFileReaderBuilder(stream)
            .UseOverrides(overrides)
            .Build();

        var beatmap = reader.ReadFile();

        //Assert
        var expected = overrides.General;
        var actual = beatmap.General;
        Assert.AreEqual(expected.Mode, actual.Mode, $"Expected to override {nameof(actual.Mode)}");
        Assert.AreEqual(expected.OsuFileFormat, actual.OsuFileFormat, $"Expected to override {nameof(actual.OsuFileFormat)}");
        Assert.AreEqual(expected.StackLeniency, actual.StackLeniency, $"Expected to override {nameof(actual.StackLeniency)}");
    }
}
