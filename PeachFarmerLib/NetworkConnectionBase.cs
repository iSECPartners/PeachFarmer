using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeachFarmerLib
{
    public abstract class NetworkConnectionBase : INetworkConnection
    {
        protected TcpClient _tcpClient;
        protected int _timeout;

        public NetworkConnectionBase(int timeout)
        {
            _tcpClient = null;
            _timeout = timeout;
        }

        public virtual void Close()
        {
            _tcpClient.Close();
            _tcpClient = null;
        }

        public void Dispose()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        public virtual Stream GetStream()
        {
            return _tcpClient.GetStream();
        }
    }
}
