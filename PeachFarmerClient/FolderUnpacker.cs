using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using PeachFarmerLib.Framework;

namespace PeachFarmerClient
{
    public class FolderUnpacker : IFolderUnpacker
    {
        private IFileSystem _fileSystem;

        public FolderUnpacker(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
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
            string outputFileName = GenerateExtractedFilename(destinationFolder, archiveEntry.FullName);
            Stream outputStream = _fileSystem.GetOutputStream(outputFileName);
            using (BinaryWriter streamWriter = new BinaryWriter(outputStream))
            {
                using (BinaryReader streamReader = new BinaryReader(archiveEntry.Open()))
                {
                    try
                    {
                        while (true)
                        {
                            streamWriter.Write(streamReader.ReadByte());
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        ;
                    }
                }
            }
        }

        protected virtual string GenerateExtractedFilename(string destinationFolder, string packagePath)
        {
            string combinedPath = Path.Combine(destinationFolder, packagePath);

            //
            // Convert the path to canonicalized form.
            //

            string fullPath = Path.GetFullPath(combinedPath);

            if (!fullPath.StartsWith(destinationFolder))
            {
                throw new ArgumentException("Invalid package path. Package files must not be located outside of root directory.");
            }

            return fullPath;
        }
    }
}
