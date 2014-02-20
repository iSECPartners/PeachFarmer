using PeachFarmerLib;
using PeachLauncher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class CommandProcessor
    {
        private string _folderPath;

        private string _connectionPassword;

        private string _serverCertFile;

        private string _clientCertFile;

        public CommandProcessor(CommandLineOptions options)
        {
            _folderPath = options.LogFolder;

            _connectionPassword = options.Password;

            _serverCertFile = options.ServerCertFile;

            _clientCertFile = options.ClientCertFile;
        }

        public void MonitorFolder()
        {
            int listenPort = PeachFarmerProtocol.FarmerPort;

            Console.WriteLine("Starting log monitor on: {0}", _folderPath);
            Console.WriteLine("Listening for connections on port {0}", listenPort.ToString());

            PeachFolderPackager packager = new PeachFolderPackager(new FileSystem());
            Launcher peachLauncher = new Launcher();

            using (NetworkServerConnection tcpServer = CreateNetworkConnection(listenPort, _serverCertFile, _clientCertFile))
            {
                RequestListener folderMontior = new RequestListener(tcpServer, packager, new Clock(), _folderPath, peachLauncher, _connectionPassword);

                folderMontior.Monitor();
            }
        }

        private NetworkServerConnection CreateNetworkConnection(int listenPort, string serverCertFile, string clientCertFile)
        {
            const int ConnectionTimeout = 5; // connection timeout (in seconds)

            if (serverCertFile != null)
            {
                byte[] serverCertData = File.ReadAllBytes(serverCertFile);
                byte[] clientCertData = null;
                if (clientCertFile != null)
                {
                    clientCertData = File.ReadAllBytes(clientCertFile);
                }

                return new NetworkSslServerConnection(listenPort, ConnectionTimeout, serverCertData, clientCertData);
            }
            else
            {
                return new NetworkServerConnection(listenPort, ConnectionTimeout);
            }
        }
    }
}
