﻿using PeachFarmerLib.Framework;
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
                        if (logDirectory.Equals(latestLogDirectory))
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
                if (!includeStatusFile && filename.EndsWith(Path.DirectorySeparatorChar + "status.txt"))
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
            if (zipEntryName.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                zipEntryName = zipEntryName.Substring(1);
            }

            ZipArchiveEntry entry = zipArchive.CreateEntry(zipEntryName);
            using (BinaryWriter streamWriter = new BinaryWriter(entry.Open()))
            {
                using (BinaryReader streamReader = new BinaryReader(_fileSystem.GetReadStream(filename)))
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