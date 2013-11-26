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
        private IFolderUnpacker _folderUnpacker;

        private DateTime _lastPullTime;

        private string _serverPassword;

        public FilePuller(IFolderUnpacker folderUnpacker, string serverPassword)
            :this(folderUnpacker, DateTime.MinValue, serverPassword)
        {
        }

        public FilePuller(IFolderUnpacker folderUnpacker, DateTime lastPullTime, string serverPassword)
        {
            _folderUnpacker = folderUnpacker;

            _lastPullTime = lastPullTime;

            _serverPassword = serverPassword;
        }

        public void Pull(Stream serverStream, string destinationFolder)
        {
            ReadRequestMessage readRequest = new ReadRequestMessage();
            readRequest.LastCheckTimeUtc = _lastPullTime;
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

            _lastPullTime = readResponse.CurrentServerTimeUtc;
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
