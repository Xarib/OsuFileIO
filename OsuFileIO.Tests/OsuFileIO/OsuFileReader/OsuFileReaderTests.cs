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
    [DataRow(tutorialFile, "new beginnings", "new beginnings", "nekodex", "nekodex", "pishifat", "tutorial", "", "", 2116202, 1011011)]
    [DataRow(taikoFile, "Amatsu Kitsune", "アマツキツネ", "YURiCa", "ユリカ", "Kyuukai", "Charlotte's Inner Oni", "初音ミク Project mirai 2", "marasy Hanatan 花たん", 1521524, 716642)]
    [DataRow(catchFile, "Fukagyaku Replace", "不可逆リプレイス", "MY FIRST STORY", "MY FIRST STORY", "Akitoshi", "Chara's Overdose", "信長協奏曲", "Nobunaga Concerto Ending TV Size Asagi Ster koliron Irreversible Chara", 767324, 342218)]
    [DataRow(maniaFile, "Miracle 5ympho X", "Miracle 5ympho X", "USAO", "USAO", "Mel", "Fullerene's 4K Black Another", "beatmania IIDX 21 SPADA", "frey sionkotori kokodoko ナウい 一年一片 frenchcore dubstep intensity fullerene kurokami gezo", 482546, 137394)]
    public void ReadMetadata_OsuFile_ReturnsMetadata(string fileName, string title, string titleUnicode, string artist, string artistUnicode, string creator, string version, string source, string tags, int beatmapID, int beatmapSetID)
    {
        //Arrange
        var reader = new OsuFileReaderBuilder(fileName).Build();

        //Act
        var actual = reader.ReadMetadata();

        //Assert
        var expected = new MetaData
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
        Assert.AreEqual(expected.Tags, actual.Tags, $"Expected the file reader to read '{nameof(expected.Tags)}' correctly");
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
        Assert.AreEqual("test", actual.Tags, $"Expected the file reader to read '{nameof(actual.Tags)}' correctly");
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
        Assert.AreEqual(actual1.MetaData, actual2.MetaData, $"Expected to get the same {nameof(MetaData)}");
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
}
