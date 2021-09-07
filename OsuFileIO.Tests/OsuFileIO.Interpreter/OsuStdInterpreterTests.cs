using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private const string shortStd = "stdShort.osu";

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


        //TODO look into this again
        [TestMethod]
        [DataRow("550,301.507537688442,4,1,0,100,1,0", "172,142,21052,2,0,P|411:58|133:279,1,1200", 24067)]
        [DataRow("550,301.507537688442,6,1,0,100,1,0", "189,100,20449,2,0,P|375:74|125:266,1,936", 22801)]
        [DataRow("550,301.507537688442,7,1,0,100,1,0", "85,82,21655,6,0,B|358:67|358:67|129:270|129:270|393:273,1,840", 23766)]
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

        private class ActualInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
        }
    }
}
