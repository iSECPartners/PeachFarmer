using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib
{
    public class FileSystem : IFileSystem
    {
        public string[] GetFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public string[] GetDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

        public DateTime GetLastModifiedTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public Stream GetOutputStream(string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            return File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public Stream GetReadStream(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
