using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.OsuFileReader.Exceptions
{
    [Serializable]
    class OsuFileReaderException : Exception
    {
        public OsuFileReaderException() { }

        public OsuFileReaderException(string message)
            : base(message) { }
    }
}
