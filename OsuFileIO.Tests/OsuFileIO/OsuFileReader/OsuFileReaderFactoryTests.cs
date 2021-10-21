using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsuFileIO.OsuFileReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Tests.OsuFileIO.OsuFileReader
{
    [TestClass]
    public class OsuFileReaderFactoryTests
    {
        private const string fileName = "stdShort.osu";
        private const string catchFile = "catch.osu";
        private const string maniaFile = "mania.osu";
        private const string taikoFile = "taiko.osu";
        private const string fileLocation = "TestFiles/";

        [TestMethod]
        [DeploymentItem(fileLocation + fileName)]
        public void Dispose_WhenNoReaderWasBuilt_ThrowsException()
        {
            //Arrange
            var stream = File.OpenRead(fileName);

            var factory = new OsuFileReaderBuilder(stream);

            //Act
            factory.Dispose();

            void actual() => stream.ReadByte();

            //Assert
            Assert.ThrowsException<ObjectDisposedException>(actual);
        }

        [TestMethod]
        [DeploymentItem(fileLocation + fileName)]
        public void Dispose_NoDisposeWhenReaderWasBuilt_DoesNotThrow()
        {
            //Arrange
            var stream = File.OpenRead(fileName);

            var factory = new OsuFileReaderBuilder(stream);
            var reader = factory.Build();

            //Act
            factory.Dispose();

            //Assert
            _ = stream.Position;
        }

        [TestMethod]
        [DeploymentItem(fileLocation + fileName)]
        [DataRow(fileName, typeof(StdFileReader))]
        [DataRow(taikoFile, typeof(TaikoFileReader))]
        [DataRow(maniaFile, typeof(ManiaFileReader))]
        [DataRow(catchFile, typeof(CatchFileReader))]
        public void Build_OsuFile_ReturnsOsuFileReader(string filePath, Type type)
        {
            //Arrange
            var stream = File.OpenRead(filePath);

            //Act
            var factory = new OsuFileReaderBuilder(stream);
            var reader = factory.Build();
            //Assert

            Assert.IsTrue(reader.GetType() == type, $"Expected the {nameof(type)} for file but got an nother reader");
        }

        [TestMethod]
        public void Constructor_NonOsuFile_ThrowsException()
        {
            //Act
            static void actual() => new OsuFileReaderBuilder("test.txt");

            //Assert
            Assert.ThrowsException<ArgumentException>(actual);
        }

        [TestMethod]
        public void Constructor_FileDoesNotExist_ThrowsException()
        {
            //Act
            static void actual() => new OsuFileReaderBuilder("test.osu");

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
            writer.Flush();
            stream.Position = 0;

            //Act
            using var reader = new OsuFileReaderBuilder(stream).Build();

            //Asset
            Assert.IsTrue(reader is StdFileReader, "Expected the osu!Standard file reader");
        }
    }
}
