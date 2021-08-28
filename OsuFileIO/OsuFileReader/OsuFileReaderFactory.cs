﻿using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.OsuFileReader.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader
{
    public class OsuFileReaderFactory : IDisposable
    {
        private OsuFileReaderOptions options;
        private OsuFileReaderOverride readerOverride;
        private readonly Stream stream;
        private bool willBeDisposedByReader;

        /// <summary>
        /// Opens and reads a .osu file and returns the corresponding reader for the given gamemode
        /// </summary>
        /// <param name="path"></param>
        public OsuFileReaderFactory([NotNull] string path, OsuFileReaderOptions options = null)
        {
            if (!path.EndsWith(".osu"))
                throw new ArgumentException("The given file is not a osu file");

            if (!File.Exists(path))
                throw new FileNotFoundException("File '" + path + "' does not exist");

            this.options = options;
            this.stream = File.OpenRead(path);
        }

        public OsuFileReaderFactory([NotNull] Stream stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void ConfigureOptions(OsuFileReaderOptions options)
        {
            this.options = options;
        }

        public void ConfigureOverride(OsuFileReaderOverride readerOverride)
        {
            this.readerOverride = readerOverride;
        }

        private static string searchString = "Mode:";
        public OsuFileReader Build()
        {
            StreamReader sr = new(this.stream);

            string line;
            if (options is null)
            {
                line = sr.ReadLineStartingWithOrNull(searchString);
            }
            else
            {
                line = sr.ReadLineStartingWithOrNull(searchString, this.options.StringComparison);
            }

            var mode = Enum.Parse<GameMode>(line
                .TrimStart()
                .Remove(0, searchString.Length)
                .Trim());

            this.stream.Position = 0;

            switch (mode)
            {
                case GameMode.Standard:
                    this.willBeDisposedByReader = true;

                    return new OsuStdFileReader(this.stream, this.options, this.readerOverride);
                case GameMode.Taiko:
                    this.willBeDisposedByReader = false; //TODO: Change this if implemented

                    throw new NotImplementedException();
                case GameMode.Catch:
                    this.willBeDisposedByReader = false; //TODO: Change this if implemented

                    throw new NotImplementedException();
                case GameMode.Mania:
                    this.willBeDisposedByReader = false; //TODO: Change this if implemented

                    throw new NotImplementedException();
                default:
                    throw new OsuFileReaderException();
            }
        }

        public void Dispose()
        {
            if (!this.willBeDisposedByReader)
                this.stream.Dispose();
        }
    }

    public enum IntParsing
    {
        Default = 0,
        ConvertFloat = 1,
    }
}
