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

        public virtual byte[] ReadBytes(int length)
        {
            Stream clientStream = GetStream();
            
            byte[] message = new byte[length];
            int bytesRead;
            int totalBytesRead = 0;
            const int maxReadSize = 1024;
            AutoResetEvent readCompleted = new AutoResetEvent(false);

            while (totalBytesRead < length)
            {
                int bytesToRequest = Math.Min(maxReadSize, length - totalBytesRead);
                bytesRead = 0;

                clientStream.BeginRead(
                    message,
                    totalBytesRead,
                    bytesToRequest,
                    delegate(IAsyncResult r)
                        {
                            try
                            {
                                bytesRead = clientStream.EndRead(r);
                            }
                            catch (ObjectDisposedException)
                            {
                                //
                                // If timeout occurred, clientStream will have already been disposed.
                                //
                            }
                            readCompleted.Set();
                        },
                    null);
                
                if (!readCompleted.WaitOne(TimeSpan.FromSeconds(_timeout), false))
                {
                    throw new TimeoutException("Network connection did not respond in time.");
                }

                if (bytesRead == 0)
                {
                    throw new Exception(string.Format("Not enough network data read. Expected {0}, got {1}.", length, totalBytesRead));
                }

                totalBytesRead += bytesRead;
            }

            return message;
        }

        public virtual void WriteBytes(byte[] data)
        {
            Stream clientStream = GetStream();

            clientStream.Write(data, 0, data.Length);
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
            }
        }

        protected virtual Stream GetStream()
        {
            return _tcpClient.GetStream();
        }
    }
}
