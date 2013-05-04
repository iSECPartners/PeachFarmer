using PeachFarmerLib;
using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class FolderMonitor
    {
        private INetworkConnection _serverConnection;

        private Stream _clientStream;

        private IFolderPacker _folderPackager;

        private IClock _clock;

        private string _folderPath;

        private byte[] _correctPasswordHash;

        public FolderMonitor(INetworkConnection serverConnection, IFolderPacker folderPackager, IClock clock, string folderPath, string password)
        {
            _serverConnection = serverConnection;
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

                _serverConnection.Close();
            }
        }

        public void ProcessNextRequest()
        {
            _clientStream = _serverConnection.GetStream();
            IFarmerNetworkMessage request = ReceiveRequest();

            switch (request.MessageType)
            {
                case PeachFarmerProtocol.ReadRequest:
                    ProcessReadRequest((ReadRequestMessage)request);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognized request type: 0x{0:X2}", request.MessageType));
            }
        }

        private bool IsPasswordValid(string sentPassword)
        {
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

        private void ProcessReadRequest(ReadRequestMessage request)
        {
            Console.WriteLine("ReadRequest from client at {0}", DateTime.Now);

            ReadResponseMessage response = new ReadResponseMessage();
            response.IsPasswordCorrect = IsPasswordValid(request.ServerPassword);
            if (!response.IsPasswordCorrect)
            {
                Console.WriteLine("Rejecting connection. Invalid password.");
                SendResponse(response);
                return;
            }

            response.CurrentServerTimeUtc = _clock.GetCurrentTimeUtc();
            response.Data = _folderPackager.PackFolder(_folderPath, request.LastCheckTimeUtc);

            Console.WriteLine("Sending {0} bytes.", response.Data.Length);

            SendResponse(response);

            Console.WriteLine("Done!");
        }

        private IFarmerNetworkMessage ReceiveRequest()
        {
            FarmerMessageSerializer deserializer = new FarmerMessageSerializer();
            return deserializer.Deserialize(_clientStream);
        }

        private void SendResponse(IFarmerNetworkMessage response)
        {
            FarmerMessageSerializer serializer = new FarmerMessageSerializer();
            serializer.Serialize(_clientStream, response);
        }
    }
}
