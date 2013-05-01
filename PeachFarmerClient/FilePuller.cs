using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class FilePuller
    {
        IDataConnection _dataConnection;

        IFolderUnpacker _folderUnpacker;

        IPullHistory _pullHistory;

        string _serverPassword;

        public FilePuller(IDataConnection dataConection, IFolderUnpacker folderUnpacker, IPullHistory pullHistory, string serverPassword)
        {
            _dataConnection = dataConection;

            _folderUnpacker = folderUnpacker;

            _pullHistory = pullHistory;

            _serverPassword = serverPassword;
        }

        public void Pull(string workerName, string destinationFolder)
        {
            _dataConnection.SendByte(PeachFarmerProtocol.ReadRequest);

            _dataConnection.SendString(_serverPassword);
            if (_dataConnection.ReceiveByte() != PeachFarmerProtocol.PasswordCorrect)
            {
                Console.WriteLine("Error: Incorrect password.");
                return;
            }

            DateTime modifiedAfterUtc = _pullHistory.GetLastPullTime(workerName);
            Console.WriteLine("Requesting files modified after {0}", modifiedAfterUtc.ToLocalTime());
            _dataConnection.SendDateTime(modifiedAfterUtc);

            DateTime checkTimeUtc = _dataConnection.ReceiveDateTime();
            Console.WriteLine("Server check time was {0}", checkTimeUtc.ToLocalTime());

            int dataLength = _dataConnection.ReceiveInt();
            Console.WriteLine("Data length is {0}", dataLength);

            byte[] packageData = _dataConnection.ReceiveBytes(dataLength);
            Console.WriteLine("Read {0} bytes", packageData.Length);

            _folderUnpacker.UnpackFolder(destinationFolder, packageData);
            _pullHistory.SetLastPullTime(workerName, checkTimeUtc);
        }
    }
}
