using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileReader
{
    [TestClass]
    public class OsuFileReaderFactoryTests
    {
        private const string fileName = "stdShort.osu";

        [TestMethod]
        [DeploymentItem("OsuFileReader/TestFiles/" + fileName)]
        public void Dispose_WhenNoReaderWasBuilt_ThrowsException()
        {
            //Arrange
            var stream = File.OpenRead(fileName);

            var factory = new OsuFileReaderFactory(stream);

            //Act
            factory.Dispose();

            void actual() => stream.ReadByte();

            //Assert
            Assert.ThrowsException<ObjectDisposedException>(actual);
        }

        [TestMethod]
        [DeploymentItem("OsuFileReader/TestFiles/" + fileName)]
        public void Dispose_NoDisposeWhenReaderWasBuilt_DoesNotThrow()
        {
            //Arrange
            var stream = File.OpenRead(fileName);

            var factory = new OsuFileReaderFactory(stream);
            var reader = factory.Build();

            //Act
            factory.Dispose();

            //Assert
            _ = stream.Position;
        }

        [TestMethod]
        [DeploymentItem("OsuFileReader/TestFiles/" + fileName)]
        public void Build_StrShortFile_ReturnsOsuStdFileReader()
        {
            //Arrange
            var stream = File.OpenRead(fileName);

            //Act
            var factory = new OsuFileReaderFactory(stream);
            var reader = factory.Build();
            //Assert

            Assert.IsTrue(reader is OsuStdFileReader, $"Expected the {nameof(OsuStdFileReader)} for file but got an nother reader");
        }

        [TestMethod]
        public void Constructor_NonOsuFile_ThrowsException()
        {
            //Act
            static void actual() => new OsuFileReaderFactory("test.txt");

            //Assert
            Assert.ThrowsException<ArgumentException>(actual);
        }

        [TestMethod]
        public void Constructor_FileDoesNotExist_ThrowsException()
        {
            //Act
            static void actual() => new OsuFileReaderFactory("test.osu");

            //Assert
            Assert.ThrowsException<FileNotFoundException>(actual);
        }

        [TestMethod]
        public void Build_ModeNotInFile_ReturnsOsuFileReader()
        {
            //Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("osu file format v14");
            writer.WriteLine("Mode: 0");
            writer.Flush();
            stream.Position = 0;

            //Act
            var reader = new OsuFileReaderFactory(stream).Build();

            //Asset
            Assert.IsTrue(reader is OsuStdFileReader, "Expected the osu!Standard file reader");
        }
    }
}
