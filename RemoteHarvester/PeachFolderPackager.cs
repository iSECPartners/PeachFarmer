using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class PeachFolderPackager : IFolderPacker
    {
        private IFileSystem _fileSystem;

        public PeachFolderPackager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public byte[] PackFolder(string sourceFolder, DateTime lastModifiedMinimumUtc)
        {
            List<string> logDirectories = FindPeachRunLogDirectories(sourceFolder);

            string latestLogDirectory = GetLatestLogDirectory(logDirectories);

            byte[] packagedData = null;

            using (MemoryStream zipStream = new MemoryStream(0))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (string logDirectory in logDirectories)
                    {
                        bool isLatest;
                        if (string.CompareOrdinal(logDirectory, latestLogDirectory) == 0)
                        {
                            isLatest = true;
                        }
                        else
                        {
                            isLatest = false;
                        }

                        List<string> filesToPackage = GetFilesToPackage(logDirectory, lastModifiedMinimumUtc, isLatest);

                        foreach (string filename in filesToPackage)
                        {
                            PackFile(logDirectory, filename, zipArchive);
                        }
                    }
                }
                packagedData = zipStream.ToArray();
            }

            return packagedData;
        }

        private List<string> GetFilesToPackage(string sourceFolder, DateTime lastModifiedMinimumUtc, bool includeStatusFile)
        {
            List<string> filesToPackage = new List<string>();
            foreach (string filename in _fileSystem.GetFiles(sourceFolder))
            {
                if (_fileSystem.GetLastModifiedTime(filename).ToUniversalTime() <= lastModifiedMinimumUtc)
                {
                    continue;
                }

                //
                // If PeachFarmer is monitoring the root of logging folder (i.e. a folder containing multiple runs of Peach logs),
                // we collect the contents of every folder, but only take the status.txt file from the latest log output.
                //

                if (!includeStatusFile && filename.EndsWith(Path.DirectorySeparatorChar + "status.txt", StringComparison.InvariantCulture))
                {
                    continue;
                }

                filesToPackage.Add(filename);
            }

            foreach (string subdirectory in _fileSystem.GetDirectories(sourceFolder))
            {
                filesToPackage.AddRange(GetFilesToPackage(subdirectory, lastModifiedMinimumUtc, true));
            }

            return filesToPackage;
        }

        private void PackFile(string folderRoot, string filename, ZipArchive zipArchive)
        {
            string zipEntryName = filename.Substring(folderRoot.Length);
            if (zipEntryName.StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.InvariantCulture))
            {
                zipEntryName = zipEntryName.Substring(1);
            }

            ZipArchiveEntry entry = zipArchive.CreateEntry(zipEntryName);

            using (Stream fileStream = _fileSystem.GetReadStream(filename))
            {
                using (Stream archiveStream = entry.Open())
                {
                    fileStream.CopyTo(archiveStream);
                }
            }
        }

        private List<string> FindPeachRunLogDirectories(string folderRoot)
        {
            List<string> logDirectories = new List<string>();

            if (_fileSystem.FileExists(Path.Combine(folderRoot, "status.txt")))
            {
                logDirectories.Add(folderRoot);
                return logDirectories;
            }

            foreach (string subDirectory in _fileSystem.GetDirectories(folderRoot))
            {
                logDirectories.AddRange(FindPeachRunLogDirectories(subDirectory));
            }

            return logDirectories;
        }

        private string GetLatestLogDirectory(IEnumerable<string> logDirectories)
        {
            string latestLogDirectory = null;
            DateTime? latestLogTime = null;

            foreach (string logDirectory in logDirectories)
            {
                DateTime logTime = _fileSystem.GetLastModifiedTime(Path.Combine(logDirectory, "status.txt"));

                if ((latestLogDirectory == null) || (logTime > latestLogTime))
                {
                    latestLogDirectory = logDirectory;
                    latestLogTime = logTime;
                }
            }

            return latestLogDirectory;
        }
    }
}
