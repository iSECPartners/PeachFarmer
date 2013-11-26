using PeachFarmerClient.Framework;
using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class NetworkClientConnection : NetworkConnectionBase, INetworkClientConnection
    {
        public NetworkClientConnection(string host, int port, int timeout)
            :base(timeout)
        {
            RemoteAddress = host;

            RemotePort = port;
        }

        private void EnsureConnected()
        {
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                Connect();
            }
        }

        private void Connect()
        {
            _tcpClient = new TcpClient();
            IAsyncResult asyncResult = _tcpClient.BeginConnect(RemoteAddress, RemotePort, null, null);
            System.Threading.WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(_timeout), false))
                {
                    _tcpClient.Close();
                    throw new TimeoutException(string.Format("Failed to connect to {0}:{1} within {2} seconds.", RemoteAddress, RemotePort, _timeout));
                }

                _tcpClient.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }
        }

        public override Stream GetStream()
        {
            EnsureConnected();

            return base.GetStream();
        }

        public string RemoteAddress { get; private set; }

        public int RemotePort {get; private set; }
    }
}
