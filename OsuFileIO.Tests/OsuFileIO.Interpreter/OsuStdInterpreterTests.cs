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

        //[TestMethod]
        //[DataRow(100d * 1d, 1, "1266,279.06976744186,4,1,9,90,1,0")]
        //[DataRow(100d * 0.7d * 1d, "34125,-100,4,1,0,87,0,0")]
        //[DataRow(0.7, "40472,-83.3333333333333,4,1,0,100,0,0", 100d * 0.7d * 1.2d)]
        //public void Interpret_HitObject_RetunrsLengthOfMap(double expected, double sliderMultiplier, string timingPoint, string hitObject)
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
