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
    public class FilePuller
    {
        private Stream _serverStream;

        private IFolderUnpacker _folderUnpacker;

        private IPullHistory _pullHistory;

        private string _serverPassword;

        public FilePuller(Stream serverStream, IFolderUnpacker folderUnpacker, IPullHistory pullHistory, string serverPassword)
        {
            _serverStream = serverStream;

            _folderUnpacker = folderUnpacker;

            _pullHistory = pullHistory;

            _serverPassword = serverPassword;
        }

        public void Pull(string workerName, string destinationFolder)
        {
            ReadRequestMessage readRequest = new ReadRequestMessage();
            readRequest.LastCheckTimeUtc = _pullHistory.GetLastPullTime(workerName);
            readRequest.ServerPassword = _serverPassword;

            FarmerMessageSerializer messageSerializer = new FarmerMessageSerializer();
            messageSerializer.Serialize(_serverStream, readRequest);

            ReadResponseMessage readResponse = (ReadResponseMessage)messageSerializer.Deserialize(_serverStream);
            if (!readResponse.IsPasswordCorrect)
            {
                Console.WriteLine("Error: Incorrect password.");
                return;
            }

            Console.WriteLine("Server check time was {0}", readResponse.CurrentServerTimeUtc.ToLocalTime());
            Console.WriteLine("Read {0} bytes", readResponse.Data.Length);

            _folderUnpacker.UnpackFolder(destinationFolder, readResponse.Data);
            _pullHistory.SetLastPullTime(workerName, readResponse.CurrentServerTimeUtc);
        }
    }
}
