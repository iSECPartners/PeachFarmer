using PeachFarmerLib;
using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;
using PeachLauncher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class RequestListener
    {
        private INetworkConnection _serverConnection;

        private ReadRequestProcessor _readRequestProcessor;

        private AssignWorkRequestProcessor _assignWorkRequestProcessor;

        private Stream _clientStream;

        public RequestListener(INetworkConnection serverConnection, IFolderPacker folderPackager, IClock clock, string folderPath, ILauncherService launcher, string password)
        {
            _serverConnection = serverConnection;

            _readRequestProcessor = new ReadRequestProcessor(folderPackager, clock, folderPath, password);

            _assignWorkRequestProcessor = new AssignWorkRequestProcessor(launcher, password);
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
            RequestMessageBase request = ReceiveRequest();

            switch (request.MessageType)
            {
                case PeachFarmerProtocol.ReadRequest:
                    {
                        Console.WriteLine("ReadRequest from client at {0}", DateTime.Now);
                        ResponseMessageBase response = _readRequestProcessor.Process(request);
                        Console.WriteLine("Sending response...");
                        SendResponse(response);
                        Console.WriteLine("Done!");
                        break;
                    }
                case PeachFarmerProtocol.AssignWorkRequest:
                    {
                        Console.WriteLine("AssignWorkRequest from client at {0}", DateTime.Now);
                        ResponseMessageBase response = _assignWorkRequestProcessor.Process(request);
                        Console.WriteLine("Sending response...");
                        SendResponse(response);
                        Console.WriteLine("Done!");
                        break;
                    }
                default:
                    throw new ArgumentException(string.Format("Unrecognized request type: 0x{0:X2}", request.MessageType));
            }
        }

        private RequestMessageBase ReceiveRequest()
        {
            FarmerMessageSerializer deserializer = new FarmerMessageSerializer();
            return (RequestMessageBase)deserializer.Deserialize(_clientStream);
        }

        private void SendResponse(IFarmerNetworkMessage response)
        {
            FarmerMessageSerializer serializer = new FarmerMessageSerializer();
            serializer.Serialize(_clientStream, response);
        }
    }
}
