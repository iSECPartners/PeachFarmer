using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class NetworkSslServerConnection : NetworkServerConnection
    {
        private SslStream _sslStream;

        private X509Certificate2 _serverCert;

        private X509Certificate2 _pinnedClientCert;

        public NetworkSslServerConnection(int port, int timeout, string serverCertFile, string clientCertFile)
            :base(port, timeout)
        {
            _serverCert = new X509Certificate2(serverCertFile);
            if (clientCertFile != null)
            {
                _pinnedClientCert = new X509Certificate2(clientCertFile);
            }
            else
            {
                _pinnedClientCert = null;
            }
            _sslStream = null;
        }

        public override Stream GetStream()
        {
            if (_sslStream == null)
            {
                _sslStream = new SslStream(base.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCertificate));

                bool requireClientCert = (_pinnedClientCert != null);

                _sslStream.AuthenticateAsServer(_serverCert, requireClientCert, SslProtocols.Tls12, false);
            }

            return _sslStream;
        }

        public bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (_pinnedClientCert == null)
            {
                return true;
            }

            if ((sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) && (_pinnedClientCert != null))
            {
                return _pinnedClientCert.GetCertHashString().Equals(certificate.GetCertHashString());
            }
            else
            {
                return (sslPolicyErrors == SslPolicyErrors.None);
            }
        }

        public override void Close()
        {
            _sslStream.Close();
            _sslStream = null;

            base.Close();
        }
    }
}
