using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class NetworkSslClientConnection : NetworkClientConnection
    {
        private X509Certificate2 _serverCert;
        private X509Certificate2 _clientCert;
        private SslStream _sslStream;

        public NetworkSslClientConnection(string host, int port, int timeout, byte[] serverCertData, byte[] clientCertData)
            :base(host, port, timeout)
        {
            _serverCert = new X509Certificate2(serverCertData);
            if (clientCertData != null)
            {
                _clientCert = new X509Certificate2(clientCertData);
            }
            else
            {
                _clientCert = null;
            }
            _sslStream = null;
        }

        public override Stream GetStream()
        {
            if (_sslStream == null)
            {
                _sslStream = new SslStream(base.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCertificate), new LocalCertificateSelectionCallback(SelectLocalCertificate));

                X509Certificate2Collection clientCertificates = new X509Certificate2Collection();
                if (_clientCert != null)
                {
                    clientCertificates.Add(_clientCert);
                }

                _sslStream.AuthenticateAsClient("PeachFarmerServer", clientCertificates, SslProtocols.Tls12, false);
            }

            return _sslStream;
        }

        public bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                return _serverCert.GetCertHashString().Equals(certificate.GetCertHashString());
            }
            else
            {
                return (sslPolicyErrors == SslPolicyErrors.None);
            }
        }

        public X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return _clientCert;
        }

        public override void Close()
        {
            _sslStream.Close();
            _sslStream = null;

            base.Close();
        }
    }
}
