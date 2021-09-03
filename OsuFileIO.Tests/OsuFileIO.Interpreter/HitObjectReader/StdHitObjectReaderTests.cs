using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.Interpreter.HitObjectReader;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.Interpreter.HitObjectReader
{
    [TestClass]
    public class StdHitObjectReaderTests
    {
        [TestMethod]
        public void ShouldGetMostCurrentTimingPoint()
        {
            //Arrange
            var timingPoints = new List<TimingPoint>
            {
                new TimingPoint
                {
                    TimeInMs = 25,
                },
                new TimingPoint
                {
                    BeatLength = 100,
                    TimeInMs = 50,
                },
                new TimingPoint
                {
                    BeatLength = 200,
                    TimeInMs = 50,
                },
                new TimingPoint
                {
                    BeatLength = 200,
                    TimeInMs = 55,
                },
            };

            var hitObjects = new List<IHitObject>
            {
                new Circle(new Coordinates(), 30),
                new Circle(new Coordinates(), 50),
            };

            var difficulty = new Difficulty();

            //Act
            var reader = new StdHitObjectReader(difficulty, timingPoints, hitObjects);

            do
            {
                ;
            } while (reader.ReadNext());

            //Assert
            Assert.AreEqual(hitObjects[1].TimeInMs, reader.CurrentTimingPoint.TimeInMs, "Expected a timing point that has a smaller or equal time of the last object");
            Assert.AreEqual(timingPoints[2].BeatLength, reader.CurrentTimingPoint.BeatLength, "Expected the most current timingPoint");
        }
    }
}
