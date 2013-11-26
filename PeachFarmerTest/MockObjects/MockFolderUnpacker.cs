using PeachFarmerClient.Framework;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    public class MockFolderUnpacker : IFolderUnpacker
    {
        public string LastDestinationFolder { get; private set; }

        public byte[] LastPackedData { get; private set; }

        public void UnpackFolder(string destinationFolder, byte[] packedData)
        {
            LastDestinationFolder = destinationFolder;

            LastPackedData = packedData;
        }

        public Stream GetStatusFileStream()
        {
            throw new NotImplementedException();
        }
    }
}
