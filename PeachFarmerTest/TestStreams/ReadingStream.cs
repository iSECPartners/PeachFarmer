using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvesterTest.TestStreams
{
    /// <summary>
    /// Subclass of MemoryStream that drops writes.
    /// </summary>
    public class ReadingStream : MemoryStream
    {
        private bool _processWrites;

        public ReadingStream()
        {
            _processWrites = true;
        }

        public void StopProcessingWrites()
        {
            _processWrites = false;
        }

        public override void WriteByte(byte value)
        {
            if (_processWrites)
            {
                base.WriteByte(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_processWrites)
            {
                base.Write(buffer, offset, count);
            }
        }
    }
}
