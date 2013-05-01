using PeachFarmerLib.Framework;
using RemoteHarvester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    class MockFolderPackager : IFolderPacker
    {
        public string LastFolderPacked { get; private set; }

        public DateTime LastModifiedMinimumRequested { get; private set; }

        public readonly static byte[] MockFolderData = new byte[] { 0x01, 0x02, 0x03 };

        public byte[] PackFolder(string sourceFolder, DateTime lastModifiedMinimum)
        {
            LastFolderPacked = sourceFolder;

            LastModifiedMinimumRequested = lastModifiedMinimum;

            return MockFolderData;
        }

        public void UnpackFolder(string destinationFolder, byte[] packedData)
        {
            throw new NotImplementedException();
        }
    }
}
