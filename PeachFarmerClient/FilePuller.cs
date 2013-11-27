using PeachFarmerClient.Framework;
using PeachFarmerLib;
using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class FilePuller : IFilePuller
    {
        public DateTime LastPullTime { get; private set; }

        private IFolderUnpacker _folderUnpacker;

        private string _serverPassword;

        public FilePuller(IFolderUnpacker folderUnpacker, string serverPassword)
            : this(folderUnpacker, serverPassword, DateTime.MinValue)
        {
        }

        public FilePuller(IFolderUnpacker folderUnpacker, string serverPassword, DateTime lastPullTime)
        {
            _folderUnpacker = folderUnpacker;

            _serverPassword = serverPassword;

            LastPullTime = lastPullTime;
        }

        public void Pull(Stream serverStream, string destinationFolder)
        {
            ReadRequestMessage readRequest = new ReadRequestMessage();
            readRequest.LastCheckTimeUtc = LastPullTime;
            readRequest.ServerPassword = _serverPassword;

            FarmerMessageSerializer messageSerializer = new FarmerMessageSerializer();
            messageSerializer.Serialize(serverStream, readRequest);

            ReadResponseMessage readResponse = (ReadResponseMessage)messageSerializer.Deserialize(serverStream);
            if (!readResponse.IsPasswordCorrect)
            {
                Console.WriteLine("Error: Incorrect password.");
                return;
            }

            Console.WriteLine("Server check time was {0}", readResponse.CurrentServerTimeUtc.ToLocalTime());
            Console.WriteLine("Read {0}", FormatByteCount(readResponse.Data.Length));

            _folderUnpacker.UnpackFolder(destinationFolder, readResponse.Data);

            LastPullTime = readResponse.CurrentServerTimeUtc;
        }

        private string FormatByteCount(int byteCount)
        {
            const int BytesPerMegabyte = 1024 * 1024;
            const int BytesPerKilobyte = 1024;

            if (byteCount > BytesPerMegabyte)
            {
                return (string.Format("{0:0.00} MB", ((float)byteCount / BytesPerMegabyte)));
            }
            else if (byteCount > BytesPerKilobyte)
            {
                return (string.Format("{0:0.00} KB", ((float)byteCount / BytesPerKilobyte)));
            }
            else
            {
                return string.Format("{0} bytes", byteCount);
            }
        }

        public Stream GetStatusFileStream()
        {
            return _folderUnpacker.GetStatusFileStream();
        }
    }
}
