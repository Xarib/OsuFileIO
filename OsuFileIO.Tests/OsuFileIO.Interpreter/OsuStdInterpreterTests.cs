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

        //[TestMethod]
        //[DataRow(, 1, "1266,279.06976744186,4,1,9,90,1,0", )]
        //public void Interpret_HitObject_RetunrsLengthOfMap(double expectedLength, double sliderMultiplier, string timingPoint, string hitObject)
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
        //    writer.WriteLine("SliderMultiplier:" + sliderMultiplier.ToString());
        //    writer.WriteLine("[TimingPoints]");
        //    writer.WriteLine(timingPoint);
        //    writer.WriteLine("[HitObjects]");
        //    writer.WriteLine("328,80,999999999,2,0,P|412:101|464:168,1,167.999994873047,4|4,0:0|0:0,0:0:0:0:");
        //    writer.Flush();
        //    stream.Position = 0;

        //    var fileReader = new OsuFileReaderFactory(stream).Build();

        //    var file = fileReader.ReadFile() as OsuStdFile;

        //    var actual = new ActualInterpretation();

        //    var interpreter = new OsuStdInterpreter(actual);
        //    interpreter.Interpret(file);
        //}

        [TestMethod]
        [DeploymentItem(fileLocation + "1172819.osu")]
        [DataRow("1172819.osu")]
        public void Interpret_RealMaps_RetunrsLengthOfMap(string fileName)
        {
            //Arrange
            var fileReader = new OsuFileReaderFactory(fileName).Build();
            var file = fileReader.ReadFile() as OsuStdFile;

            //Act
            var actual = new ActualInterpretation();
            var interpreter = new OsuStdInterpreter(actual);
            interpreter.Interpret(file);

            //Assert
        }

        private class ActualInterpretation : IInterpretation
        {
            public TimeSpan Length { get; set; }
        }
    }
}
