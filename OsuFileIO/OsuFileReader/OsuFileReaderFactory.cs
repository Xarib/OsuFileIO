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
    public class OsuFileReaderFactory
    {
        private readonly string path;
        private readonly OsuFileReaderFactoryOptions options;

        /// <summary>
        /// Opens and reads a .osu file and returns the corresponding reader for the given gamemode
        /// </summary>
        /// <param name="path"></param>
        public OsuFileReaderFactory([NotNull] string path, OsuFileReaderFactoryOptions options = null)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.options = options;

            if (!path.EndsWith(".osu"))
                throw new ArgumentException("The given file is not a osu file");

            if (!File.Exists(path))
                throw new FileNotFoundException("File '" + path + "' does not exist");
        }

        private static string searchString = "Mode:";
        public OsuFileReader Build()
        {
            using StreamReader sr = new(path);

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

            return mode switch
            {
                GameMode.Standard => new OsuStdFileReader(),
                GameMode.Taiko => throw new NotImplementedException(),
                GameMode.Catch => throw new NotImplementedException(),
                GameMode.Mania => throw new NotImplementedException(),
                _ => throw new OsuFileReaderException(),
            };
        }
    }

    public class OsuFileReaderFactoryOptions
    {
        public StringComparison StringComparison { get; set; }
    }
}
