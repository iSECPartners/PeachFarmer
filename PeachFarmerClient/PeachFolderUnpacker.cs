using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class PeachFolderUnpacker : FolderUnpacker
    {
        private string _disambiguatorName;

        public PeachFolderUnpacker(IFileSystem fileSystem, string disambiguatorName)
            :base(fileSystem)
        {
            _disambiguatorName = disambiguatorName;
        }

        protected override string GenerateExtractedFilename(string destinationFolder, string packagePath)
        {
            if ((string.CompareOrdinal(packagePath, "status.txt") == 0) || 
                packagePath.EndsWith(Path.DirectorySeparatorChar + "status.txt", StringComparison.InvariantCulture))
            {
                string adjustedPackagePath = string.Format("status-{0}.txt", _disambiguatorName);

                return base.GenerateExtractedFilename(destinationFolder, adjustedPackagePath);
            }
            else
            {
                return base.GenerateExtractedFilename(destinationFolder, packagePath);
            }
        } 
    }
}
