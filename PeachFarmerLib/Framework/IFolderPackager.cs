using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface IFolderPacker
    {
        byte[] PackFolder(string sourceFolder, DateTime lastModifiedMinimum);
    }
}
