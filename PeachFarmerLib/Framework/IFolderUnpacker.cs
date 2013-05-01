using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface IFolderUnpacker
    {
        void UnpackFolder(string destinationFolder, byte[] packedData);
    }
}
