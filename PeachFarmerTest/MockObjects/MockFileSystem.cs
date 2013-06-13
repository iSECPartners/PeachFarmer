using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    public class MockFileSystem : IFileSystem
    {
        Dictionary<string, MockFile> _files;

        public MockFileSystem()
        {
            _files = new Dictionary<string, MockFile>();
        }

        #region IFileSystem implementation
        public string[] GetFiles(string directoryPath)
        {
            List<string> directoryFiles = new List<string>();

            int rootFolderEnd = directoryPath.Length;
            if (!directoryPath.EndsWith("\\"))
            {
                rootFolderEnd++;
            }

            foreach (string filename in _files.Keys)
            {

                //
                // Don't treat it as a file in this directory if it's within a subfolder.
                //

                if ((filename.Length < rootFolderEnd) ||
                    (filename.IndexOf("\\", rootFolderEnd) > 0))
                {
                    continue;
                }

                if (filename.StartsWith(directoryPath, StringComparison.InvariantCulture))
                {
                    directoryFiles.Add(filename);
                }
            }

            return directoryFiles.ToArray();
        }

        public string[] GetDirectories(string directoryPath)
        {
            HashSet<string> directories = new HashSet<string>();

            string slashEndedPath = directoryPath;
            if (!directoryPath.EndsWith("\\"))
            {
                slashEndedPath += "\\";
            }

            foreach (string filename in _files.Keys)
            {
                if (!filename.StartsWith(slashEndedPath))
                {
                    continue;
                }

                int subDirectoryEnd = filename.IndexOf("\\", slashEndedPath.Length);
                if (subDirectoryEnd < 0)
                {
                    continue;
                }

                string subDirectory = filename.Substring(0, subDirectoryEnd);
                directories.Add(subDirectory);
            }

            return directories.ToArray();
        }

        public DateTime GetLastModifiedTime(string path)
        {
            try
            {
                return _files[path].LastModifiedTime;
            }
            catch (KeyNotFoundException)
            {
                throw new FileNotFoundException("Could not find mock file", path);
            }
        }

        public Stream GetOutputStream(string filePath)
        {
            if (!_files.ContainsKey(filePath))
            {
                MockFile newFile = new MockFile(filePath);
                AddMockFile(newFile);
            }
            
            return _files[filePath].DataStream;
        }

        public Stream GetReadStream(string filePath)
        {
            try
            {
                MemoryStream readStream = _files[filePath].DataStream;
                readStream.Seek(0, SeekOrigin.Begin);

                return readStream;
            }
            catch (KeyNotFoundException)
            {
                throw new FileNotFoundException("Could not find mock file", filePath);
            }
        }
        #endregion

        public void AddMockFile(MockFile file)
        {
            _files.Add(file.Filename, file);
        }

        public void AddMockFile(string filename, DateTime lastModifiedTime, string mockData)
        {
            MockFile mockFile = new MockFile(filename, lastModifiedTime);
            BinaryWriter writer = new BinaryWriter(mockFile.DataStream);
            byte[] mockBytes = Encoding.ASCII.GetBytes(mockData);
            writer.Write(mockBytes);
            mockFile.DataStream.Seek(0, SeekOrigin.Begin);

            AddMockFile(mockFile);
        }

        public bool FileExists(string filename)
        {
            return _files.ContainsKey(filename);
        }

        public int GetTotalFileCount()
        {
            return _files.Keys.Count;
        }

        public override bool Equals(object obj)
        {
            return Equals((MockFileSystem)obj);
        }

        public bool Equals(MockFileSystem other)
        {
            if (this._files.Count != other._files.Count)
            {
                return false;
            }

            foreach (string filename in _files.Keys)
            {
                MockFile file = _files[filename];
                MockFile otherFile = _files[filename];

                if (!file.Equals(otherFile))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
