using OsuFileIO.Enums;
using OsuFileIO.Extensions;
using OsuFileIO.HitObject;
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
    public class OsuFileReaderBuilder : IDisposable
    {
        private OsuFileReaderOptions options;
        private OsuFileReaderOverride readerOverride;
        private bool willBeDisposedByReader;
        private readonly StreamReader sr;

        /// <summary>
        /// Opens and reads a .osu file and returns the corresponding reader for the given gamemode
        /// </summary>
        /// <param name="path"></param>
        public OsuFileReaderBuilder([NotNull] string path)
        {
            if (!path.EndsWith(".osu"))
                throw new ArgumentException("The given file is not a osu file");

            if (!File.Exists(path))
                throw new FileNotFoundException("File '" + path + "' does not exist");

            this.sr = new StreamReader(path);
        }

        public OsuFileReaderBuilder([NotNull] Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            this.sr = new StreamReader(stream);
        }

        public OsuFileReaderBuilder UseOptions(OsuFileReaderOptions options)
        {
            this.options = options;

            return this;
        }

        public OsuFileReaderBuilder UseOverrides(OsuFileReaderOverride readerOverride)
        {
            this.readerOverride = readerOverride;

            return this;
        }

        private const string searchString = "Mode:";
        public IOsuFileReader<IHitObject> Build()
        {
            string line;
            if (options is null)
            {
                line = sr.ReadLineStartingWithOrNull(searchString, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                line = sr.ReadLineStartingWithOrNull(searchString, this.options.StringComparison);
            }

            GameMode mode;
            if (line is null)
            {
                mode = GameMode.Standard;
                if (this.readerOverride is null)
                {
                    this.readerOverride = new OsuFileReaderOverride
                    {
                        General = new OsuFile.General
                        {
                            Mode = mode,
                        }
                    };
                }
                else if (this.readerOverride.General is null)
                {
                    this.readerOverride.General = new OsuFile.General
                    {
                        Mode = mode,
                    };
                }
                else
                {
                    this.readerOverride.General.Mode = mode;
                }
            }
            else
            {
                mode = Enum.Parse<GameMode>(line
                .TrimStart()
                .Remove(0, searchString.Length)
                .Trim());
            }

            this.sr.Reset();

            switch (mode)
            {
                case GameMode.Standard:
                    this.willBeDisposedByReader = true;

                    return new StdFileReader(this.sr, this.options, this.readerOverride);
                case GameMode.Taiko:
                    this.willBeDisposedByReader = true;

                    return new TaikoFileReader(this.sr, this.options, this.readerOverride);
                case GameMode.Catch:
                    this.willBeDisposedByReader = true;

                    return new CatchFileReader(this.sr, this.options, this.readerOverride);
                case GameMode.Mania:
                    this.willBeDisposedByReader = true;

                    return new ManiaFileReader(this.sr, this.options, this.readerOverride);
                default:
                    this.Dispose();
                    throw new OsuFileReaderException();
            }
        }

        public void Dispose()
        {
            if (!this.willBeDisposedByReader)
            {
                this.sr.Dispose();
            }
        }
    }

    public enum IntParsing
    {
        Default = 0,
        ConvertFloat = 1,
    }
}
