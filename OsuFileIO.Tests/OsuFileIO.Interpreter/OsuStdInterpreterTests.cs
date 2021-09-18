using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.Interpreter;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.Interpreter
{
    [TestClass]
    public class OsuStdInterpreterTests
    {
        private const string fileLocation = "TestFiles/";
        private const string tutorialFile = "new beginnings.osu";

        #region slider length
        [TestMethod]
        [DataRow("479,194,31356,1,4,0:0:0:0:", 31356)]
        [DataRow("256,192,126433,12,4,129202,3:2:0:0:", 129202)]
        public void Interpret_EndsWithCircleOrSpinner_RetunrsLengthOfMap(string hitObject, int expectedLength)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(TimeSpan.FromMilliseconds(expectedLength), actual.Length, "Expected to interpret the length correclty");
        }

        [TestMethod]
        [DataRow("550,301.507537688442,4,1,0,100,1,0", "172,142,21052,2,0,P|411:58|133:279,1,1200", 24067)]
        [DataRow("550,301.507537688442,4,1,0,100,1,0", "189,100,20449,2,0,P|375:74|125:266,1,936", 22801)]
        [DataRow("550,301.507537688442,4,1,0,100,1,0", "85,82,21655,6,0,B|358:67|358:67|129:270|129:270|393:273,1,840", 23766)]
        public void Interpret_Meter_RetunrsLengthOfMap(string timingPoint, string hitObject, double expectedEndTime)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();

            var file = fileReader.ReadFile() as OsuStdFile;

            var actual = new ActualInterpretation();

            //Act
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            var milisecondDifference = Math.Abs(actual.Length.TotalMilliseconds - expectedEndTime);
            Assert.IsTrue(milisecondDifference < 1, $"Expected to calculate the slider end time correctly with max 1ms difference but got {milisecondDifference}ms");
        }

        [TestMethod]
        [DeploymentItem(fileLocation + "1172819.osu")]
        [DeploymentItem(fileLocation + "1860169.osu")]
        [DataRow("1172819.osu", 305271)]
        [DataRow("1860169.osu", 431132)]
        public void Interpret_RealMaps_RetunrsLengthOfMap(string fileName, int endtimeInMs)
        {
            //Arrange
            var fileReader = new OsuFileReaderFactory(fileName).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            var milisecondDifference = Math.Abs(actual.Length.TotalMilliseconds - endtimeInMs);
            Assert.IsTrue(milisecondDifference < 1, "Expected to calculate the slider end time correctly with max 1ms difference");
        }
        #endregion

        #region Hitobject count

        [TestMethod]
        [DataRow(4)]
        [DataRow(42)]
        public void Interpret_CirclesOnly_ReturnsHitCircleCount(int count)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(count, actual.HitCircleCount, $"Expected to count HitCircles correctly");
        }

        [TestMethod]
        [DataRow(4)]
        [DataRow(42)]
        public void Interpret_SlidersOnly_ReturnsHitSliderCount(int count)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(count, actual.SliderCount, $"Expected to count Sliders correctly");
        }

        [TestMethod]
        [DataRow(4)]
        [DataRow(42)]
        public void Interpret_SpinnersOnly_ReturnsHitSjpinnerCount(int count)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

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
        public void Interpret_ActualMaps_ReturnsHitObjectCount(string fileName, int expectedCircleCount, int expectedSliderCount, int expectedSpinnerCount)
        {
            //Arrange
            var fileReader = new OsuFileReaderFactory(fileName).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCircleCount, actual.HitCircleCount, $"Expected to count HitCircles correctly");
            Assert.AreEqual(expectedSliderCount, actual.SliderCount, $"Expected to count Sliders correctly");
            Assert.AreEqual(expectedSpinnerCount, actual.SpinnerCount, $"Expected to count Spinners correctly");
        }
        #endregion

        #region Bpm
        /*
         * Bpm
         * 100Bpm = 600
         * 150Bpm = 400
         * 200Bpm = 300
         * 250Bpm = 240
         */
        //TODO make this work
        //[TestMethod]
        //[DataRow(new ValueTuple<int, double>[] { new ValueTuple<int, double>(100, 300) })]
        //public void Interpret_ActualMaps_ReturnBpm(ValueTuple<int, double>[] timingPoints ,double bpm, double bpmMin, double bpmMax)
        //{
        //    //Arrange
        //    var stream = new MemoryStream();
        //    var writer = new StreamWriter(stream);
        //    writer.WriteLine("osu file format v14");
        //    writer.WriteLine("[General]");
        //    writer.WriteLine("StackLeniency: 0.7");
        //    writer.WriteLine("Mode: 0");
        //    writer.WriteLine("[Metadata]");
        //    writer.WriteLine("[Difficulty]");
        //    writer.WriteLine("SliderMultiplier: 0.7");
        //    writer.WriteLine("[TimingPoints]");

        //    int timePassed = 0;
        //    foreach (var timing in timingPoints)
        //    {
        //        writer.WriteLine($"{timing.Item1},{timing.Item2},4,1,9,90,1,0");
        //        timePassed = timing.Item1;
        //    }

        //    writer.WriteLine("[HitObjects]");
        //    writer.WriteLine($"479,194,{timePassed},1,4,0:0:0:0:");
        //    writer.Flush();
        //    stream.Position = 0;

        //    var fileReader = new OsuFileReaderFactory(stream).Build();
        //    var file = fileReader.ReadFile() as OsuStdFile;

        //    //Act
        //    var actual = new ActualInterpretation();
        //    var interpreter = new OsuStdInterpreter(actual);
        //    interpreter.Interpret(file);

        //    //Assert
        //}

        [TestMethod]
        [DeploymentItem(fileLocation + "1172819.osu")]
        [DeploymentItem(fileLocation + tutorialFile)]
        [DataRow("1172819.osu", 258, 103, 260)]
        [DataRow(tutorialFile, 130, 130, 130)]
        public void Interpret_ActualMaps_ReturnBpm(string fileName, double bpm, double bpmMin, double bpmMax)
        {
            //Arrange
            var fileReader = new OsuFileReaderFactory(fileName).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

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
        public void Interpret_VariousHitObjects_ReturnsDoubleCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.DoubleCount, "Expected to count doubles correctly");
            Assert.AreEqual(expectedCount, actual.TrueDoubleCount, "Expected to count true doubles correctly");
        }

        [TestMethod]
        [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
        [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
        [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
        public void Interpret_VariousSliderLengths_ReturnsTrueDoubleCount(string slider, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.TrueDoubleCount, "Expected to count true doubles correctly");
        }

        [TestMethod]
        [DataRow(new int[] { 3 }, 1)]
        [DataRow(new int[] { 3, 3 }, 2)]
        [DataRow(new int[] { 3, 4 }, 1)]
        [DataRow(new int[] { 0, 4 }, 0)]
        public void Interpret_VariousHitObjects_ReturnsTripletCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.TripletCount, "Expected to count triplets correctly");
            Assert.AreEqual(expectedCount, actual.TrueTripletCount, "Expected to count true triplets correctly");
        }

        [TestMethod]
        [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
        [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
        [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
        public void Interpret_VariousSliderLengths_ReturnsTrueTripletCount(string slider, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.TrueTripletCount, "Expected to count true doubles correctly");
        }

        [TestMethod]
        [DataRow(new int[] { 4 }, 1)]
        [DataRow(new int[] { 4, 4 }, 2)]
        [DataRow(new int[] { 3, 4 }, 1)]
        [DataRow(new int[] { 3, 5 }, 0)]
        public void Interpret_VariousHitObjects_ReturnsQuadrupletCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.QuadrupletCount, "Expected to count quadruplets correctly");
            Assert.AreEqual(expectedCount, actual.TrueQuadrupletCount, "Expected to count true quadruplets correctly");
        }

        [TestMethod]
        [DataRow("163,150,22258,6,0,L|236:149,1,63.7500002235173", 1)]
        [DataRow("163,150,22258,2,0,L|253:150,1,85.0000002980232", 0)]
        [DataRow("163,150,22258,6,0,L|275:152,1,106.250000372529", 0)]
        public void Interpret_VariousSliderLengths_ReturnsTrueQuadrupletCount(string slider, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.TrueQuadrupletCount, "Expected to count true doubles correctly");
        }

        #endregion

        #region StreamCounting

        [TestMethod]
        [DataRow(600, new int[] { 20 }, 0)] //100Bmp
        [DataRow(400, new int[] { 5 }, 0)] //150Bmp
        [DataRow(400, new int[] { 20 }, 20)] //150Bmp
        [DataRow(300, new int[] { 5 }, 0)] //200Bmp
        [DataRow(300, new int[] { 5, 20 }, 20)] //200Bmp
        [DataRow(300, new int[] { 5, 20, 10 }, 20)] //200Bmp
        public void Interpret_VariousStreams_ReturnsLongestStreamCount(double beatLength, int[] streamsLengths, int expectedLength)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

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
        public void Interpret_VariousStreams200Bpm_ReturnsBurstCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

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
        public void Interpret_VariousStreams200Bpm_ReturnsStreamCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.StreamCount, $"Expected count {nameof(actual.StreamCount)} correctly");
        }

        [TestMethod]
        [DataRow(new int[] { 16 }, 0)]
        [DataRow(new int[] { 33 }, 0)]
        [DataRow(new int[] { 24 }, 1)]
        [DataRow(new int[] { 21, 22, 23 }, 3)]
        public void Interpret_VariousStreams200Bpm_ReturnsLongStreamCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.LongStreamCount, $"Expected count {nameof(actual.LongStreamCount)} correctly");
        }

        [TestMethod]
        [DataRow(new int[] { 30 }, 0)]
        [DataRow(new int[] { 33 }, 1)]
        [DataRow(new int[] { 56, 34, 35 }, 3)]
        public void Interpret_VariousStreams200Bpm_ReturnsDeathStreamCount(int[] streamsLengths, int expectedCount)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedCount, actual.DeathStreamCount, $"Expected count {nameof(actual.DeathStreamCount)} correctly");
        }

        [TestMethod]
        [DataRow(60000d / 310d, 0)]//310Bpm 
        [DataRow(60000d / 400d, 0)]//310Bpm 
        public void Interpret_300BpmPlusBmpOneTwoJumps_ReturnsLongestStreamCount(double beatLength, int expectedLength)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

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
        public void Interpret_StreamWithSpacing_ReturnsStreamPixels(int x, int y, int hitObjectCount, double expectedPixels)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedPixels, actual.TotalStreamAlikePixels, $"Expected to calculate {actual.TotalStreamAlikePixels}");
        }

        [TestMethod]
        public void Interpret_MultipleStreamWithSpacing_ReturnsStreamPixels()
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(5 * count, actual.TotalStreamAlikePixels, $"Expected to calculate {nameof(actual.TotalStreamAlikePixels)} correctly");
        }

        #endregion

        #region SpacedStreamPixels

        [TestMethod]
        [DataRow(-1, 5, 0)]
        [DataRow(1, 5, 4)]
        [DataRow(3, 5, 12)]
        public void Interpret_SpacedStream_ReturnsSpacedStreamPixels(int spacePixels, int hitObjectCount, double expectedSpacePixels)
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
            Assert.AreEqual(expectedSpacePixels, Convert.ToInt32(actual.TotalSpacedStreamAlikePixels), $"Expected to calculate {nameof(actual.TotalSpacedStreamAlikePixels)} correctly");
        }

        #endregion

        #region Miscellaneous

        [TestMethod]
        public void Interpret_TimingPointAndInheritedPointBeforeHitObject_ShouldNotThrowError()
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

            var fileReader = new OsuFileReaderFactory(stream).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);
        }

        #endregion

        private class ActualInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
            public int HitCircleCount { get; set; }
            public int SliderCount { get; set; }
            public int SpinnerCount { get; set; }
            public double Bpm { get; set; }
            public double BpmMin { get; set; }
            public double BpmMax { get; set; }
            public int DoubleCount { get; set; }
            public int TrueDoubleCount { get; set; }
            public int TripletCount { get; set; }
            public int TrueTripletCount { get; set; }
            public int QuadrupletCount { get; set; }
            public int TrueQuadrupletCount { get; set; }
            public int BurstCount { get; set; }
            public int StreamCount { get; set; }
            public int LongStreamCount { get; set; }
            public int DeathStreamCount { get; set; }
            public int LongestStream { get; set; }
            public double TotalStreamAlikePixels { get; set; }
            public double TotalSpacedStreamAlikePixels { get; set; }
            public int JumpStreamCount { get; set; }
        }
    }
}
