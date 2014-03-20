using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class ConnectionManager : IConnectionManager
    {
        private byte[] _serverCertData;
        private byte[] _clientCertData;

        public ConnectionManager()
            :this(null, null)
        {
        }

        public ConnectionManager(byte[] serverCertData, byte[] clientCertData)
        {
            _serverCertData = serverCertData;
            _clientCertData = clientCertData;
        }

        public NetworkClientConnection CreateNetworkConnection(string host, int listenPort)
        {
            const int ConnectionTimeoutInSeconds = 30;

            if (_serverCertData != null)
            {
                return new NetworkSslClientConnection(host, listenPort, ConnectionTimeoutInSeconds, _serverCertData, _clientCertData);
            }
            else
            {
                return new NetworkClientConnection(host, listenPort, ConnectionTimeoutInSeconds);
            }
        }
    }
}
