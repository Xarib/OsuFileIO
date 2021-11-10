using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.HitObject.OsuStd;
using OsuFileIO.Analyzer;
using OsuFileIO.Analyzer.Result;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.Analyzer;

[TestClass]
public class StdAnalyzerTests
{
    private const string fileLocation = "TestFiles/";
    private const string tutorialFile = "new beginnings.osu";

    #region slider length
    [TestMethod]
    [DataRow("479,194,31356,1,4,0:0:0:0:", 31356)]
    [DataRow("256,192,126433,12,4,129202,3:2:0:0:", 129202)]
    public void Analyze_EndsWithCircleOrSpinner_RetunrsLengthOfMap(string hitObject, int expectedLength)
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
        writer.WriteLine(hitObject);
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(TimeSpan.FromMilliseconds(expectedLength), actual.Length, "Expected to analyze the length correclty");
    }

    [TestMethod]
    [DataRow("550,301.507537688442,4,1,0,100,1,0", "172,142,21052,2,0,P|411:58|133:279,1,1200", 24067)]
    [DataRow("550,301.507537688442,4,1,0,100,1,0", "189,100,20449,2,0,P|375:74|125:266,1,936", 22801)]
    [DataRow("550,301.507537688442,4,1,0,100,1,0", "85,82,21655,6,0,B|358:67|358:67|129:270|129:270|393:273,1,840", 23766)]
    [DataRow("550,301.507537688442,4,2,1,60,1,0", "29,192,550,2,0,L|150:192,2,120.000000596046", 1153)]
    [DataRow("550,301.507537688442,4,2,1,60,1,0", "29,192,550,2,0,L|150:192,4,120.000000596046", 1756)]
    public void Analyze_Meter_RetunrsLengthOfMap(string timingPoint, string hitObject, double expectedEndTime)
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
        writer.WriteLine("SliderMultiplier:1.2");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine(timingPoint);
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(hitObject);
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();

        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        var actual = new ActualAnalysis();

        //Act
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        var milisecondDifference = Math.Abs(actual.Length.TotalMilliseconds - expectedEndTime);
        Assert.IsTrue(milisecondDifference < 1, $"Expected to calculate the slider end time correctly with max 1ms difference but got {milisecondDifference}ms");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + "1172819.osu")]
    [DeploymentItem(fileLocation + "1860169.osu")]
    [DataRow("1172819.osu", 305271)]
    [DataRow("1860169.osu", 431132)]
    public void Analyze_RealMaps_RetunrsLengthOfMap(string fileName, int endtimeInMs)
    {
        //Arrange
        var fileReader = new OsuFileReaderBuilder(fileName).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        var milisecondDifference = Math.Abs(actual.Length.TotalMilliseconds - endtimeInMs);
        Assert.IsTrue(milisecondDifference < 1, "Expected to calculate the slider end time correctly with max 1ms difference");
    }
    #endregion

    #region Hitobject count

    [TestMethod]
    [DataRow(4)]
    [DataRow(42)]
    public void Analyze_CirclesOnly_ReturnsHitCircleCount(int count)
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

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine("63,279,104741,1,2,0:3:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.HitCircleCount, $"Expected to count HitCircles correctly");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(42)]
    public void Analyze_SlidersOnly_ReturnsHitSliderCount(int count)
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

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine("351,36,108894,6,0,L|394:373,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.SliderCount, $"Expected to count Sliders correctly");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(42)]
    public void Analyze_SpinnersOnly_ReturnsHitSjpinnerCount(int count)
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

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine("256,192,126433,12,4,129202,3:2:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.SpinnerCount, $"Expected to count Spinners correctly");
    }

    [TestMethod]
    [DeploymentItem(fileLocation + "1172819.osu")]
    [DeploymentItem(fileLocation + "1860169.osu")]
    [DeploymentItem(fileLocation + tutorialFile)]
    [DataRow("1172819.osu", 1441, 460, 4)]
    [DataRow("1860169.osu", 2232, 604, 1)]
    [DataRow(tutorialFile, 20, 12, 2)]
    public void Analyze_ActualMaps_ReturnsHitObjectCount(string fileName, int expectedCircleCount, int expectedSliderCount, int expectedSpinnerCount)
    {
        //Arrange
        var fileReader = new OsuFileReaderBuilder(fileName).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCircleCount, actual.HitCircleCount, $"Expected to count HitCircles correctly");
        Assert.AreEqual(expectedSliderCount, actual.SliderCount, $"Expected to count Sliders correctly");
        Assert.AreEqual(expectedSpinnerCount, actual.SpinnerCount, $"Expected to count Spinners correctly");
    }
    #endregion

    #region Bpm

    [TestMethod]
    [DeploymentItem(fileLocation + "1172819.osu")]
    [DeploymentItem(fileLocation + tutorialFile)]
    [DataRow("1172819.osu", 258, 103, 260)]
    [DataRow(tutorialFile, 130, 130, 130)]
    public void Analyze_ActualMaps_ReturnBpm(string fileName, double bpm, double bpmMin, double bpmMax)
    {
        //Arrange
        var fileReader = new OsuFileReaderBuilder(fileName).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.AreEqual(bpm, actual.Bpm, $"Expected to get the correct {actual.Bpm}");
        Assert.AreEqual(bpmMin, actual.BpmMin, $"Expected to get the correct {actual.BpmMin}");
        Assert.AreEqual(bpmMax, actual.BpmMax, $"Expected to get the correct {actual.BpmMax}");
    }

    #endregion

    #region SubStreamCounting

    [TestMethod]
    [DataRow(new int[] { 2 }, 1)]
    [DataRow(new int[] { 2, 2 }, 2)]
    [DataRow(new int[] { 2, 4 }, 1)]
    [DataRow(new int[] { 0, 4 }, 0)]
    public void Analyze_VariousHitObjects_ReturnsDoubleCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.DoubleCount, "Expected to count doubles correctly");
        Assert.AreEqual(expectedCount, actual.StandaloneDoubleCount, "Expected to count true doubles correctly");
    }

    [TestMethod]
    [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
    [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
    [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
    public void Analyze_VariousSliderLengths_ReturnsTrueDoubleCount(string slider, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"550,301.507537688442,4,2,1,60,1,0");
        writer.WriteLine($"21052,-200,4,2,1,60,0,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(slider);
        writer.WriteLine("290,152,22635,1,0,0:0:0:0:");
        writer.WriteLine("313,154,22710,1,0,0:0:0:0:");
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.StandaloneDoubleCount, "Expected to count true doubles correctly");
    }

    [TestMethod]
    [DataRow(new int[] { 3 }, 1)]
    [DataRow(new int[] { 3, 3 }, 2)]
    [DataRow(new int[] { 3, 4 }, 1)]
    [DataRow(new int[] { 0, 4 }, 0)]
    public void Analyze_VariousHitObjects_ReturnsTripletCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.TripletCount, "Expected to count triplets correctly");
        Assert.AreEqual(expectedCount, actual.StandaloneTripletCount, "Expected to count true triplets correctly");
    }

    [TestMethod]
    [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
    [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
    [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
    public void Analyze_VariousSliderLengths_ReturnsTrueTripletCount(string slider, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"550,301.507537688442,4,2,1,60,1,0");
        writer.WriteLine($"21052,-200,4,2,1,60,0,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(slider);
        writer.WriteLine("290,152,22635,1,0,0:0:0:0:");
        writer.WriteLine("313,154,22710,1,0,0:0:0:0:");
        writer.WriteLine("333,155,22786,1,0,0:0:0:0:");
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.StandaloneTripletCount, "Expected to count true doubles correctly");
    }

    [TestMethod]
    [DataRow(new int[] { 4 }, 1)]
    [DataRow(new int[] { 4, 4 }, 2)]
    [DataRow(new int[] { 3, 4 }, 1)]
    [DataRow(new int[] { 3, 5 }, 0)]
    public void Analyze_VariousHitObjects_ReturnsQuadrupletCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.QuadrupletCount, "Expected to count quadruplets correctly");
        Assert.AreEqual(expectedCount, actual.StandaloneQuadrupletCount, "Expected to count true quadruplets correctly");
    }

    [TestMethod]
    [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
    [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
    [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
    public void Analyze_VariousSliderLengths_ReturnsTrueQuadrupletCount(string slider, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"550,301.507537688442,4,2,1,60,1,0");
        writer.WriteLine($"21052,-200,4,2,1,60,0,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(slider);
        writer.WriteLine("290,152,22635,1,0,0:0:0:0:");
        writer.WriteLine("313,154,22710,1,0,0:0:0:0:");
        writer.WriteLine("333,155,22786,1,0,0:0:0:0:");
        writer.WriteLine("349,155,22861,1,0,0:0:0:0:");
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.StandaloneQuadrupletCount, "Expected to count true doubles correctly");
    }

    #endregion

    #region StreamCounting

    [TestMethod]
    [DataRow(601, new int[] { 20 }, 0)] // less than 100Bmp
    [DataRow(600, new int[] { 20 }, 20)] //100Bmp
    [DataRow(400, new int[] { 5 }, 0)] //150Bmp
    [DataRow(400, new int[] { 20 }, 20)] //150Bmp
    [DataRow(300, new int[] { 5 }, 0)] //200Bmp
    [DataRow(300, new int[] { 5, 20 }, 20)] //200Bmp
    [DataRow(300, new int[] { 5, 20, 10 }, 20)] //200Bmp
    public void Analyze_VariousStreams_ReturnsLongestStreamCount(double beatLength, int[] streamsLengths, int expectedLength)
    {
        if (!streamsLengths.All(length => length > 4))
            Assert.Fail("For this test only use length > 4");

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedLength, actual.LongestStream, "Expected to find the longest stream");
        Assert.IsTrue(actual.DoubleCount == 0, "Expected no doubles");
        Assert.IsTrue(actual.TripletCount == 0, "Expected no triplets");
        Assert.IsTrue(actual.QuadrupletCount == 0, "Expected no quadruplets");
    }

    [TestMethod]
    [DataRow(new int[] { 4 }, 0)]
    [DataRow(new int[] { 9 }, 0)]
    [DataRow(new int[] { 6 }, 1)]
    [DataRow(new int[] { 5, 6, 7 }, 3)]
    public void Analyze_VariousStreams200Bpm_ReturnsBurstCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.BurstCount, $"Expected count {nameof(actual.BurstCount)} correctly");
    }

    [TestMethod]
    [DataRow(new int[] { 10 }, 1)]
    [DataRow(new int[] { 10, 5 }, 1)]
    [DataRow(new int[] { 10, 5, 16 }, 2)]
    [DataRow(new int[] { 8, 5, 16 }, 1)]
    [DataRow(new int[] { 10, 5, 17 }, 1)]
    [DataRow(new int[] { 11, 12, 13, 14 }, 4)]
    public void Analyze_VariousStreams200Bpm_ReturnsStreamCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.StreamCount, $"Expected count {nameof(actual.StreamCount)} correctly");
    }

    [TestMethod]
    [DataRow(new int[] { 16 }, 0)]
    [DataRow(new int[] { 33 }, 0)]
    [DataRow(new int[] { 24 }, 1)]
    [DataRow(new int[] { 21, 22, 23 }, 3)]
    public void Analyze_VariousStreams200Bpm_ReturnsLongStreamCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.LongStreamCount, $"Expected count {nameof(actual.LongStreamCount)} correctly");
    }

    [TestMethod]
    [DataRow(new int[] { 30 }, 0)]
    [DataRow(new int[] { 33 }, 1)]
    [DataRow(new int[] { 56, 34, 35 }, 3)]
    public void Analyze_VariousStreams200Bpm_ReturnsDeathStreamCount(int[] streamsLengths, int expectedCount)
    {
        //Arrange
        var beatLength = 300;

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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;
        foreach (var length in streamsLengths)
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
                timePassed += beatLength / 4;
            }

            timePassed += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.DeathStreamCount, $"Expected count {nameof(actual.DeathStreamCount)} correctly");
    }

    [TestMethod]
    [DataRow(60000d / 310d, 0)]//310Bpm 
    [DataRow(60000d / 400d, 0)]//310Bpm 
    public void Analyze_300BpmPlusBmpOneTwoJumps_ReturnsLongestStreamCount(double beatLength, int expectedLength)
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
        writer.WriteLine($"-28,{beatLength},4,1,9,90,1,0");
        writer.WriteLine("[HitObjects]");

        double timePassed = 0;

        for (int i = 0; i < 10; i++)
        {
            writer.WriteLine($"63,279,{Math.Round(timePassed, 0, MidpointRounding.AwayFromZero)},1,2,0:3:0:0:");
            timePassed += beatLength / 2;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedLength, actual.LongestStream, "Expected to find the longest stream");
    }

    #endregion

    #region StreamAlikePixels

    [TestMethod]
    [DataRow(3, 4, 4, 0)]
    [DataRow(3, 4, 5, 5)]
    [DataRow(-3, 4, 5, 5)]
    [DataRow(3, -4, 5, 5)]
    [DataRow(-3, -4, 5, 5)]
    [DataRow(0, 5, 5, 5)]
    [DataRow(5, 0, 5, 5)]
    public void Analyze_StreamWithSpacing_ReturnsStreamPixels(int x, int y, int hitObjectCount, double expectedPixels)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        hitObjectCount--;
        for (int i = 0; i < hitObjectCount; i++)
        {
            writer.WriteLine("0,0,0,1,0,0:0:0:0:");
        }

        writer.WriteLine($"{x},{y},0,1,0,0:0:0:0:");
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedPixels, actual.TotalStreamAlikePixels, $"Expected to calculate {actual.TotalStreamAlikePixels}");
    }

    [TestMethod]
    public void Analyze_MultipleStreamWithSpacing_ReturnsStreamPixels()
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        var passedTime = 0;
        var count = 2;
        for (int i = 0; i < count; i++)
        {
            writer.WriteLine($"{3},{4},{passedTime},1,0,0:0:0:0:");

            writer.WriteLine($"0,0,{passedTime},1,0,0:0:0:0:");
            writer.WriteLine($"0,0,{passedTime},1,0,0:0:0:0:");
            writer.WriteLine($"0,0,{passedTime},1,0,0:0:0:0:");
            writer.WriteLine($"0,0,{passedTime},1,0,0:0:0:0:");

            passedTime += 10000;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(5 * count, actual.TotalStreamAlikePixels, $"Expected to calculate {nameof(actual.TotalStreamAlikePixels)} correctly");
    }

    #endregion

    #region SpacedStreamPixels

    [TestMethod]
    [DataRow(-1, 5, 0)]
    [DataRow(1, 5, 4)]
    [DataRow(3, 5, 12)]
    public void Analyze_SpacedStream_ReturnsSpacedStreamPixels(int spacePixels, int hitObjectCount, double expectedSpacePixels)
    {
        //Arrange
        var cs10pxRadius = 555d / 56d;

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[Metadata]");
        writer.WriteLine("[Difficulty]");
        writer.WriteLine("CircleSize:" + cs10pxRadius);
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        var x = 0;
        for (int i = 0; i < hitObjectCount; i++)
        {
            writer.WriteLine($"{x},0,0,1,0,0:0:0:0:");
            x += 10 * 2 + spacePixels;
        }
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedSpacePixels, Convert.ToInt32(actual.TotalSpacedStreamAlikePixels), $"Expected to calculate {nameof(actual.TotalSpacedStreamAlikePixels)} correctly");
    }

    #endregion

    #region StreamMiscellaneous

    [TestMethod]
    [DeploymentItem(fileLocation + "2371698_mod.osu")]
    public void Analyze_MapsWithStreamJumps_ReturnsStreamJumpCount()
    {
        //Arrange
        var fileReader = new OsuFileReaderBuilder("2371698_mod.osu").Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        var d = file.TimingPoints.Where(tp => tp is InheritedPoint).Select(tp => tp as InheritedPoint).Select(tp => tp.VelocityMultiplier).ToList();
        var s = string.Join(", ", d);

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.AreEqual(1, actual.StreamCutsCount, "Expected one stream jump");
    }

    [TestMethod]
    [DataRow(5, 1)]
    [DataRow(4, 1)]
    [DataRow(3, 0)]
    public void Analyze_JumpStreamWithVariousJumpLength_ReturnsStreamJumpCount(int jumpLengthPixels, double expectedCuts)
    {
        //Arrange
        var cs10pxRadius = 555d / 56d;

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[Metadata]");
        writer.WriteLine("[Difficulty]");
        writer.WriteLine("CircleSize:" + cs10pxRadius);
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        var x = 0;
        for (int i = 0; i < 4; i++)
        {
            writer.WriteLine($"{x},0,0,1,0,0:0:0:0:");
            x += 10 * 2;
        }

        x += jumpLengthPixels;

        for (int i = 0; i < 4; i++)
        {
            writer.WriteLine($"{x},0,0,1,0,0:0:0:0:");
            x += 10 * 2;
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCuts, actual.StreamCutsCount, "Expected to count cuts in streams");
    }

    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(2, 2)]
    [DataRow(7, 7)]
    public void Analyze_StreamWithSliders_ReturnsSlidersInStreams(int sliderCount, int epxtedCount)
    {
        //Arrange
        var bpm200 = 300;

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.WriteLine("osu file format v14");
        writer.WriteLine("[General]");
        writer.WriteLine("StackLeniency: 0.7");
        writer.WriteLine("Mode: 0");
        writer.WriteLine("[Metadata]");
        writer.WriteLine("[Difficulty]");
        writer.WriteLine("CircleSize:4");
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,{bpm200},4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("95,66,75,5,4,0:0:0:0:");
        writer.WriteLine("95,66,150,5,4,0:0:0:0:");
        writer.WriteLine("95,66,225,5,4,0:0:0:0:");

        var timePassed = 225;

        for (int i = 0; i < sliderCount; i++)
        {
            timePassed += bpm200 / 4;
            writer.WriteLine($"333,159,{timePassed},6,0,P|256:40|175:157,1,335.999989746094,4|4,0:0|0:2,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(epxtedCount, actual.SlidersInStreamAlike, "Expected to count sliders in streams");
    }

    #endregion

    #region DegreesCounting

    [TestMethod]
    [DataRow(100, 100, 1)]
    [DataRow(100, -100, 1)]
    [DataRow(100, 10, 0)]
    [DataRow(0, 50, 0)]
    public void Analyze_90DegreeJumps_ReturnsCount90DegreeJumps(int x, int y, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");
        writer.WriteLine("100,0,0,5,4,0:0:0:0:");
        writer.WriteLine($"{x},{y},0,5,4,0:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.AreEqual(expectedCount, actual.Jump90DegreesCount, "Expected to find all 90 Degree jumps");
    }

    [TestMethod]
    [DataRow(0, 0, 1)]
    [DataRow(0, 90, 0)]
    [DataRow(100, 100, 0)]
    [DataRow(100, -100, 0)]
    public void Analyze_180DegreeJumps_ReturnsCount180DegreeJumps(int x, int y, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");
        writer.WriteLine("100,0,0,5,4,0:0:0:0:");
        writer.WriteLine($"{x},{y},0,5,4,0:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;


        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.AreEqual(expectedCount, actual.Jump180DegreesCount, "Expected to find all 180 Degree jumps");
    }

    [TestMethod]
    public void Analyze_FirstJumpToLong_ReturnsCount180DegreeJumps()
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");
        writer.WriteLine("100,0,1000,5,4,0:0:0:0:");
        writer.WriteLine("0,0,1000,5,4,0:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.IsTrue(actual.Jump180DegreesCount == 0, "Expected no 180 Degree jumps");
    }

    [TestMethod]
    public void Analyze_SecondJumpToLong_ReturnsCount180DegreeJumps()
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");
        writer.WriteLine("100,0,0,5,4,0:0:0:0:");
        writer.WriteLine("0,0,1000,5,4,0:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        Assert.IsTrue(actual.Jump180DegreesCount == 0, "Expected no 180 Degree jumps");
    }

    #endregion

    #region Jumps

    [TestMethod]
    [DataRow(new int[] { 0, 350 }, 1)]
    [DataRow(new int[] { 0, 350, 0 }, 2)]
    [DataRow(new int[] { 0, 100, 0, 360 }, 1)]
    [DataRow(new int[] { 0, 400, 0, 360 }, 3)]
    public void Analyze_CrossScreenJump_ReturnsCrossScreenJumps(int[] xCoords, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        foreach (var x in xCoords)
        {
            writer.WriteLine($"{x},0,0,5,4,0:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.CrossScreenJumpCount, "Expected find all cross screen jumps");
    }

    [TestMethod]
    [DataRow(new int[] { 0, 350 }, 350)]
    [DataRow(new int[] { 0, 350, 100 }, 600)]
    public void Analyze_AnyJumps_ReturnsTotalJumpPixels(int[] xCoords, double expectedLength)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        foreach (var x in xCoords)
        {
            writer.WriteLine($"{x},0,0,5,4,0:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedLength, actual.TotalJumpPixels, "Expected to get Jump length");
    }

    [TestMethod]
    [DataRow(120, 120)]
    public void Analyze_JumpFromSlider_ReturnsTotalJumpPixels(int xCoord, double expectedLength)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine($"-120,0,550,2,0,L|0:0,4,120.000000596046");
        writer.WriteLine($"{xCoord},0,0,5,4,0:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedLength, actual.TotalJumpPixels, "Expected to calculate jump pixels from slider end");
    }

    [TestMethod]
    [DataRow(new int[] { 10 }, 1)]
    [DataRow(new int[] { 10, 0 }, 1)]
    [DataRow(new int[] { 10, -10, 0, 30 }, 3)] // Jumps: 10px -20px 10px 30px
    public void Analyze_VariousJumpLengths(int[] xCoords, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");

        foreach (var x in xCoords)
        {
            writer.WriteLine($"{x},0,0,5,4,0:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.UniqueDistancesCount, "Expected to find all unique jumps lenghts");
    }

    #endregion

    #region Sliders

    [TestMethod]
    [DataRow(new double[] { 349, 3, 44 })]
    [DataRow(new double[] { 10, 35.55, 343434 })]
    public void Analyze_SlidersWithVariousLengths_ReturnsTotalSliderLength(double[] sliderLengths)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        foreach (var length in sliderLengths)
        {
            writer.WriteLine($"0,0,10,6,0,L|394:373,1,{length},4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(sliderLengths.Sum(), actual.TotalSliderLength, "Expected to sum up all slider lengths");
    }

    [TestMethod]
    [DataRow(new int[] { 5, 7 })]
    [DataRow(new int[] { 2, 6, 5, 7 })]
    public void Analyze_SlidersWithVariousSliderPoints_ReturnsSliderPointCount(int[] sliderPoints)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        foreach (var pointCount in sliderPoints)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < pointCount; i++)
            {
                sb.Append($"|{i}:{i}");
            }

            writer.WriteLine("0,0,10,6,0,L" + sb.ToString() + ",1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(sliderPoints.Sum(), actual.SliderPointCount, "Expected to sum up all slider points");
    }

    [TestMethod]
    [DataRow(new int[] { 5, 7 })]
    [DataRow(new int[] { 2, 6, 5 })]
    [DataRow(new int[] { 2, 6, 5, 7 })]
    public void Analyze_SlidersWithVariousSliderPoints_AvgSliderPointCount(int[] sliderPoints)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        foreach (var pointCount in sliderPoints)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < pointCount; i++)
            {
                sb.Append($"|{i}:{i}");
            }

            writer.WriteLine("0,0,10,6,0,L" + sb.ToString() + ",1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual((double)sliderPoints.Sum() / sliderPoints.Length, actual.AvgSliderPointCount, "Expected to get avg slider point count");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(8)]
    public void Analyze_BèzierSliders_ReturnsBèzierSliderCount(int count)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine($"0,0,10,6,0,B|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.BèzierSliderCount, "Expected to count bèzier sliders");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(8)]
    public void Analyze_CatmullSliders_ReturnsCatmullSliderCount(int count)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine($"0,0,10,6,0,C|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.CatmullSliderCount, "Expected to count catmull sliders");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(8)]
    public void Analyze_LinearSliders_ReturnsLinearSliderCount(int count)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine($"0,0,10,6,0,L|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.LinearSliderCount, "Expected to count linear sliders");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(8)]
    public void Analyze_PerfectCicleSliders_ReturnsPerfectCicleSliderCount(int count)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");

        for (int i = 0; i < count; i++)
        {
            writer.WriteLine($"0,0,10,6,0,P|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
        }

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(count, actual.PerfectCicleSliderCount, "Expected to count perfect circle sliders");
    }

    [TestMethod]
    [DataRow(34, 0)] // 1/8
    [DataRow(68, 1)] // 1/4
    [DataRow(100, 0)]
    public void Analyze_KickSliders_ReturnsKickSliderCount(int length, int expectedCount)
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
        writer.WriteLine("SliderMultiplier: 1.6");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("1,-58.8235294117647,4,2,1,55,0,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine($"0,0,10,6,0,P|394:373,1,{length},4|4,0:0|0:3,3:0:0:0:");

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(expectedCount, actual.KickSliderCount, "Should count kicksliders correclty");
    }

    [TestMethod]
    [DataRow()]
    public void Analyze_VariousSliderAtSpeeds_AvgFasterSliderSpeed()
    {
        var sliders = new List<(int count, double speed)>()
            {
                (3, -100),
                (10, -80),
                (12, -120),
                (2, -50),
            };

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
        writer.WriteLine("SliderMultiplier: 1.6");


        var timePassed = 0;
        var timingPointString = new StringBuilder();
        var hitObjectString = new StringBuilder();

        foreach (var (count, speed) in sliders)
        {
            timingPointString.AppendLine($"{timePassed},{speed},4,2,1,55,0,0");

            for (int i = 0; i < count; i++)
            {
                hitObjectString.AppendLine($"0,0,{timePassed},6,0,L|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
                timePassed += 100;
            }

            timePassed += 10000;
        }

        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine(timingPointString.ToString());
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(hitObjectString.ToString());

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(100d * 1.6d * (-100d / -80d), actual.AvgFasterSliderSpeed, "Expected to get most common faster slider speed");
    }

    [TestMethod]
    public void Analyze_VariousSliderAtSpeeds_SliderSpeedDifference()
    {
        var sliders = new List<(int count, double speed)>()
            {
                (3, -100),
                (10, -80),
                (12, -120),
                (2, -50),
            };

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
        writer.WriteLine("SliderMultiplier: 1.6");


        var timePassed = 0;
        var timingPointString = new StringBuilder();
        var hitObjectString = new StringBuilder();

        foreach (var (count, speed) in sliders)
        {
            timingPointString.AppendLine($"{timePassed},{speed},4,2,1,55,0,0");

            for (int i = 0; i < count; i++)
            {
                hitObjectString.AppendLine($"0,0,{timePassed},6,0,L|394:373,1,10,4|4,0:0|0:3,3:0:0:0:");
                timePassed += 100;
            }

            timePassed += 10000;
        }

        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine(timingPointString.ToString());
        writer.WriteLine("[HitObjects]");
        writer.WriteLine(hitObjectString.ToString());

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        var fastestSpeeds = sliders
            .Where(s => s.count >= 10)
            .Select(s => 100d * 1.6d * (-100d / s.speed))
            .OrderByDescending(s => s)
            .ToList();

        //Assert
        Assert.AreEqual(fastestSpeeds[0] - fastestSpeeds.Last(), actual.SliderSpeedDifference, "Expected to slider speed difference of most common slider speeds");
    }

    #endregion

    #region Miscellaneous

    [TestMethod]
    [DataRow("0,0,0,5,4,0:0:0:0:", 1)]
    [DataRow("0,1,0,5,4,0:0:0:0:", 0)]
    [DataRow("0,0,0,12,4,0,3:2:0:0:", 0)]
    [DataRow("0,0,0,6,0,L|0:0,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 0)]
    public void Analyze_CirclePerfectStack_ReturnCirclePerfectStackCount(string hitObject, int prefectStackCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,5,4,0:0:0:0:");
        writer.WriteLine(hitObject);

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(prefectStackCount, actual.CirclePerfectStackCount, "Expected to count perfect circle stacks correctly");
    }

    [TestMethod]
    [DataRow("0,0,0,6,0,L|10:10,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 1)]
    [DataRow("10,10,0,6,0,L|0:0,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 1)]
    [DataRow("0,1,0,6,0,L|10:10,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 0)]
    [DataRow("0,0,0,6,0,L|10:11,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 0)]
    [DataRow("10,11,0,6,0,L|0:0,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 0)]
    [DataRow("10,10,0,6,0,L|0:10,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:", 0)]
    [DataRow("0,0,0,5,4,0:0:0:0:", 0)]
    [DataRow("0,0,0,12,4,0,3:2:0:0:", 0)]
    public void Analyze_SliderPerfectStack_ReturnCirclePerfectStackCount(string hitObject, int prefectStackCount)
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"0,300,4,2,1,60,1,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("0,0,0,6,0,L|10:10,1,335.999989746094,4|4,0:0|0:3,3:0:0:0:");
        writer.WriteLine(hitObject);

        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);

        //Assert
        Assert.AreEqual(prefectStackCount, actual.SliderPerfectStackCount, "Expected to count perfect slider stacks correctly");
    }

    [TestMethod]
    public void Analyze_TimingPointAndInheritedPointBeforeHitObject_ShouldNotThrowError()
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
        writer.WriteLine("SliderMultiplier: 1.7");
        writer.WriteLine("[TimingPoints]");
        writer.WriteLine($"550,301.507537688442,4,2,1,60,1,0");
        writer.WriteLine($"21052,-200,4,2,1,60,0,0");
        writer.WriteLine("[HitObjects]");
        writer.WriteLine("290,152,22635,1,0,0:0:0:0:");
        writer.Flush();
        stream.Position = 0;

        var fileReader = new OsuFileReaderBuilder(stream).Build();
        var file = fileReader.ReadFile() as ReadOnlyBeatmap<StdHitObject>;

        //Act
        var actual = new ActualAnalysis();
        var analyzer = new StdAnalyzer(actual);
        analyzer.Analyze(file);
    }

    #endregion

    internal class ActualAnalysis : IStdAnalysis
    {
        public TimeSpan Length { get; set; }
        public int HitCircleCount { get; set; }
        public int SliderCount { get; set; }
        public int SpinnerCount { get; set; }
        public double Bpm { get; set; }
        public double BpmMin { get; set; }
        public double BpmMax { get; set; }
        public int DoubleCount { get; set; }
        public int StandaloneDoubleCount { get; set; }
        public int TripletCount { get; set; }
        public int StandaloneTripletCount { get; set; }
        public int QuadrupletCount { get; set; }
        public int StandaloneQuadrupletCount { get; set; }
        public int BurstCount { get; set; }
        public int StreamCount { get; set; }
        public int LongStreamCount { get; set; }
        public int DeathStreamCount { get; set; }
        public int LongestStream { get; set; }
        public double TotalStreamAlikePixels { get; set; }
        public double TotalSpacedStreamAlikePixels { get; set; }
        public int StreamCutsCount { get; set; }
        public int SlidersInStreamAlike { get; set; }
        public int Jump90DegreesCount { get; set; }
        public int Jump180DegreesCount { get; set; }
        public double TotalJumpPixels { get; set; }
        public int CrossScreenJumpCount { get; set; }
        public double TotalSliderLength { get; set; }
        public int SliderPointCount { get; set; }
        public int BèzierSliderCount { get; set; }
        public int CatmullSliderCount { get; set; }
        public int LinearSliderCount { get; set; }
        public int PerfectCicleSliderCount { get; set; }
        public double AvgSliderPointCount { get; set; }
        public int KickSliderCount { get; set; }
        public double AvgFasterSliderSpeed { get; set; }
        public double SliderSpeedDifference { get; set; }
        public int CirclePerfectStackCount { get; set; }
        public int SliderPerfectStackCount { get; set; }
        public int UniqueDistancesCount { get; set; }
    }
}
