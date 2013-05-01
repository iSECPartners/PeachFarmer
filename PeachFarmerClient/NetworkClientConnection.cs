using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class NetworkClientConnection : NetworkConnectionBase
    {
        private string _host;

        private int _port;

        public NetworkClientConnection(string host, int port, int timeout)
            :base(timeout)
        {
            _host = host;

            _port = port;
        }

        private void EnsureConnected()
        {
            if (_tcpClient == null)
            {
                Connect();
            }
        }

        private void Connect()
        {
            _tcpClient = new TcpClient();
            IAsyncResult asyncResult = _tcpClient.BeginConnect(_host, _port, null, null);
            System.Threading.WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
            try
            {
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(_timeout), false))
                {
                    _tcpClient.Close();
                    throw new TimeoutException(string.Format("Failed to connect to {0}:{1} within {2} seconds.", _host, _port, _timeout));
                }

                _tcpClient.EndConnect(asyncResult);
            }
            finally
            {
                waitHandle.Close();
            }
        }

        public override byte[] ReadBytes(int length)
        {
            EnsureConnected();

            return base.ReadBytes(length);
        }

        public override void WriteBytes(byte[] data)
        {
            EnsureConnected();

            base.WriteBytes(data);
        }
    }
}
