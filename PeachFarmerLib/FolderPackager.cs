using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using PeachFarmerLib.Framework;

namespace PeachFarmerLib
{
    public class FolderPackager : IFolderPacker, IFolderUnpacker
    {
        private IFileSystem _FileSystem;

        public FolderPackager(IFileSystem fileSystem)
        {
            _FileSystem = fileSystem;
        }

        public byte[] PackFolder(string sourceFolder, DateTime lastModifiedMinimumUtc)
        {
            List<string> filesToPackage = GetFilesToPackage(sourceFolder, lastModifiedMinimumUtc);

            byte[] packagedData = null;

            using (MemoryStream zipStream = new MemoryStream(0))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (string filename in filesToPackage)
                    {
                        PackFile(sourceFolder, filename, zipArchive);
                    }
                }
                packagedData = zipStream.ToArray();
            }

            return packagedData;
        }

        private ZipArchiveEntry PackFile(string folderRoot, string filename, ZipArchive zipArchive)
        {
            string zipEntryName = filename.Substring(folderRoot.Length);
            ZipArchiveEntry entry = zipArchive.CreateEntry(zipEntryName);
            using (BinaryWriter streamWriter = new BinaryWriter(entry.Open()))
            {
                using (BinaryReader streamReader = new BinaryReader(_FileSystem.GetReadStream(filename)))
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

            return entry;
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
            Stream outputStream = _FileSystem.GetOutputStream(outputFileName);
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
            return Path.Combine(destinationFolder, packagePath);
        }

        private List<string> GetFilesToPackage(string sourceFolder, DateTime lastModifiedMinimumUtc)
        {
            List<string> filesToPackage = new List<string>();
            foreach (string filename in _FileSystem.GetFiles(sourceFolder))
            {
                if (_FileSystem.GetLastModifiedTime(filename).ToUniversalTime() <= lastModifiedMinimumUtc)
                {
                    continue;
                }

                filesToPackage.Add(filename);
            }

            foreach (string subdirectory in _FileSystem.GetDirectories(sourceFolder))
            {
                filesToPackage.AddRange(GetFilesToPackage(subdirectory, lastModifiedMinimumUtc));
            }

            return filesToPackage;
        }
    }
}
