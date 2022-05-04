using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shiorose
{
    internal class EncodingTextReader : IDisposable
    {
        private Stream _stream;

        private Encoding _encoding;
        public Encoding Encoding { get => _encoding; set => _encoding = value; }

        internal EncodingTextReader(Stream stream)
        {
            _stream = stream;
        }

        public string ReadLine()
        {
            var rawByteList = new List<byte>();
            byte[] buffer = new byte[1];

            string nl = Environment.NewLine;
            int counter = 0;
            do
            {
                _stream.Read(buffer, 0, 1);
                rawByteList.Add(buffer[0]);
                counter++;
            } while ((counter < Environment.NewLine.Length) || !_encoding.GetString(rawByteList.ToArray()).EndsWith(nl));

            return _encoding.GetString(rawByteList.GetRange(0, counter - Environment.NewLine.Length).ToArray());
        }

        public void Dispose()
        {
            ((IDisposable)_stream).Dispose();
        }
    }
}
