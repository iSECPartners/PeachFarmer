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
        private Stream _stream;

        public MockNetworkConnection(Stream stream)
        {
            _stream = stream;
        }

        public Stream GetStream()
        {
            return _stream;
        }

        public void Close()
        {
            _stream.Close();
        }

        public void Dispose()
        {
            ;
        }
    }
}
