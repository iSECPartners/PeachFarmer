using PeachFarmerLib.Framework;
using RemoteHarvester;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    class MockNetworkConnection : INetworkConnection
    {
        private byte[] _readQueue;
        private int _readQueuePosition;

        private MemoryStream _writtenData;

        public byte[] WrittenBytes
        {
            get
            {
                return _writtenData.ToArray();
            }
        }

        public MockNetworkConnection(byte[] readQueue)
        {
            _readQueue = readQueue;
            _readQueuePosition = 0;
            _writtenData = new MemoryStream();
        }

        public byte[] ReadBytes(int length)
        {
            byte[] readBuffer = new byte[length];

            Array.Copy(_readQueue, _readQueuePosition, readBuffer, 0, length);
            _readQueuePosition += length;

            return readBuffer;
        }

        public void WriteBytes(byte[] data)
        {
            _writtenData.Write(data, 0, data.Length);
        }

        public void Close()
        {
            ;
        }

        public void Dispose()
        {
            _writtenData.Dispose();
        }
    }
}
