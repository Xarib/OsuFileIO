using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader.Exceptions
{
    public class OsuFileReaderException : Exception
    {
        public OsuFileReaderException() { }

        public OsuFileReaderException(string message)
            : base(message) { }

        public OsuFileReaderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
