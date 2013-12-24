using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient.Framework
{
    public interface IFolderUnpacker
    {
        void UnpackFolder(string destinationFolder, byte[] packedData);

        Stream GetStatusFileStream();
    }
}
