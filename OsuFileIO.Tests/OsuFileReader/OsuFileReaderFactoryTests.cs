﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            Action actual = () => stream.ReadByte();

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
    }
}
