using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using OsuFileIO.OsuFileReader.HitObjectReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileReader.HitObjectReader
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

            //Act
            var reader = new StdHitObjectReader(timingPoints, hitObjects);

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
