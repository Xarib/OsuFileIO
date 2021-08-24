using OsuFileIO.Enums;
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
        private OsuFileReaderFactoryOptions options;
        private OsuFileReaderOverride readerOverride;
        private readonly Stream stream;

        /// <summary>
        /// Opens and reads a .osu file and returns the corresponding reader for the given gamemode
        /// </summary>
        /// <param name="path"></param>
        public OsuFileReaderFactory([NotNull] string path, OsuFileReaderFactoryOptions options = null)
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

        public void ConfigureOptions(OsuFileReaderFactoryOptions options)
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
            using StreamReader sr = new(this.stream);

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

            //TODO change this. Did this because of stream disposing. Dispose method cannot know if the stream is already disposed in the file reader. 
            var newStream = new MemoryStream();
            this.stream.CopyTo(newStream);
            newStream.Position = 0;
            return mode switch
            {
                GameMode.Standard => new OsuStdFileReader(newStream),
                GameMode.Taiko => throw new NotImplementedException(),
                GameMode.Catch => throw new NotImplementedException(),
                GameMode.Mania => throw new NotImplementedException(),
                _ => throw new OsuFileReaderException(),
            };
        }

        public void Dispose()
        {
            this.stream.Dispose();
        }
    }

    public class OsuFileReaderFactoryOptions
    {
        public StringComparison StringComparison { get; set; }
    }

    public class OsuFileReaderOverride
    {

    }
}
