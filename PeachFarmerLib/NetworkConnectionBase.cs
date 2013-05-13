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

        ~NetworkConnectionBase()
        {
            Dispose(false);
        }

        public virtual void Close()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public virtual Stream GetStream()
        {
            return _tcpClient.GetStream();
        }
    }
}
