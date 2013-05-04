using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class NetworkServerConnection : NetworkConnectionBase
    {
        private TcpListener _tcpListener;

        public NetworkServerConnection(int port, int timeout)
            :base(timeout)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpClient = null;
        }

        public override Stream GetStream()
        {
            if (_tcpClient == null)
            {
                _tcpListener.Start();
                _tcpClient = _tcpListener.AcceptTcpClient();
            }
            else if (!_tcpClient.Connected)
            {
                _tcpClient = _tcpListener.AcceptTcpClient();
            }

            return base.GetStream();
        }

    }
}
