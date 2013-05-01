using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class FolderMonitor
    {
        private IDataConnection _dataConnection;

        private IFolderPacker _folderPackager;

        private IClock _clock;

        private string _folderPath;

        private byte[] _correctPasswordHash;

        public FolderMonitor(IDataConnection networkConnection, IFolderPacker folderPackager, IClock clock, string folderPath, string password)
        {
            _dataConnection = networkConnection;
            _folderPackager = folderPackager;
            _clock = clock;
            _folderPath = folderPath;

            if (password != null)
            {
                _correctPasswordHash = PasswordHasher.CalculateHash(password);
            }
            else
            {
                _correctPasswordHash = null;
            }
        }

        public void Monitor()
        {
            while (true)
            {
                try
                {
                    ProcessNextRequest();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception processing message: {0}\r\n{1}", ex.Message, ex.StackTrace.ToString());
                }

                _dataConnection.Close();
            }
        }

        public void ProcessNextRequest()
        {
            byte requestType = _dataConnection.ReceiveByte();

            if (!IsPasswordValid())
            {
                Console.WriteLine("Rejecting connection. Invalid password.");
                _dataConnection.SendByte(PeachFarmerProtocol.PasswordIncorrect);
                return;
            }

            _dataConnection.SendByte(PeachFarmerProtocol.PasswordCorrect);

            switch (requestType)
            {
                case PeachFarmerProtocol.ReadRequest:
                    ProcessReadRequest();
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognized request type: 0x{0:X2}", requestType));
            }
        }

        private bool IsPasswordValid()
        {
            const int MaxPasswordLength = 128;
            string sentPassword = _dataConnection.ReceiveString(MaxPasswordLength);
            if (_correctPasswordHash == null)
            {
                return true;
            }

            if (sentPassword == null)
            {
                return false;
            }

            byte[] sentPasswordHash = PasswordHasher.CalculateHash(sentPassword);

            for (int i = 0; i < _correctPasswordHash.Length; i++)
            {
                if (_correctPasswordHash[i] != sentPasswordHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void ProcessReadRequest()
        {
            Console.WriteLine("ReadRequest from client at {0}", DateTime.Now);

            DateTime minModifiedTimeUtc = _dataConnection.ReceiveDateTime();

            _dataConnection.SendDateTime(_clock.GetCurrentTimeUtc());

            byte[] folderBytes = _folderPackager.PackFolder(_folderPath, minModifiedTimeUtc);

            byte[] folderBytesLength = BitConverter.GetBytes(folderBytes.Length);

            Console.WriteLine("Sending package length: {0} bytes", folderBytes.Length);
            _dataConnection.SendInt(folderBytes.Length);

            Console.WriteLine("Sending package bytes...");
            _dataConnection.SendBytes(folderBytes);
            Console.WriteLine("Done!");
        }
    }
}
