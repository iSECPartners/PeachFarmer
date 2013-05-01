using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface IFileSystem
    {
        string[] GetFiles(string directoryPath);

        string[] GetDirectories(string directoryPath);

        DateTime GetLastModifiedTime(string path);

        Stream GetOutputStream(string filePath);

        Stream GetReadStream(string filePath);

        bool FileExists(string filePath);
    }
}
