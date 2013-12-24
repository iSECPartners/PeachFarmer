using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using PeachFarmerClient.Framework;
using PeachFarmerLib.Framework;

namespace PeachFarmerClient
{
    public class FolderUnpacker : IFolderUnpacker
    {
        private IFileSystem _fileSystem;
        private string _disambiguatorName;
        private string _statusFilePath;

        public FolderUnpacker(IFileSystem fileSystem, string disambiguatorName)
        {
            _fileSystem = fileSystem;
            _disambiguatorName = disambiguatorName;
        }

        public void UnpackFolder(string destinationFolder, byte[] packedData)
        {
            using (ZipArchive zipArchive = new ZipArchive(new MemoryStream(packedData), ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    ExtractPackEntry(entry, destinationFolder);
                }
            }
        }

        private void ExtractPackEntry(ZipArchiveEntry archiveEntry, string destinationFolder)
        {
            string outputFilename;
            bool isStatusFile;

            GenerateExtractedFilename(destinationFolder, archiveEntry.FullName, out outputFilename, out isStatusFile);
            using (Stream sourceStream = archiveEntry.Open())
            {
                using (Stream destinationStream = _fileSystem.GetOutputStream(outputFilename))
                {
                    sourceStream.CopyTo(destinationStream);
                }
            }

            if (isStatusFile)
            {
                _statusFilePath = outputFilename;
            }
        }

        protected void GenerateExtractedFilename(string destinationFolder, string packagePath, out string outputFilename, out bool isStatusFile)
        {
            string fixedPackagePath;

            if ((string.CompareOrdinal(packagePath, "status.txt") == 0) ||
                packagePath.EndsWith(Path.DirectorySeparatorChar + "status.txt", StringComparison.InvariantCulture))
            {
                fixedPackagePath = string.Format("status-{0}.txt", _disambiguatorName);
                isStatusFile = true;
            }
            else
            {
                fixedPackagePath = packagePath;
                isStatusFile = false;
            }

            string combinedPath = Path.Combine(destinationFolder, fixedPackagePath);

            //
            // Convert the path to canonicalized form.
            //

            string fullPath = Path.GetFullPath(combinedPath);

            if (!fullPath.StartsWith(destinationFolder, StringComparison.InvariantCulture))
            {
                throw new ArgumentException("Invalid package path. Package files must not be located outside of root directory.");
            }

            outputFilename = fullPath;
        }

        public Stream GetStatusFileStream()
        {
            if (_statusFilePath == null)
            {
                return null;
            }

            return _fileSystem.GetReadStream(_statusFilePath);
        }
    }
}
