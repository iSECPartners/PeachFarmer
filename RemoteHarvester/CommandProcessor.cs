using PeachFarmerLib;
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
        public void Process(CommandLineOptions options)
        {
            DoMonitorFolder(options.LogFolder, PeachFarmerProtocol.FarmerPort, options.Password, options.ServerCertFile, options.ClientCertFile);
        }

        private void DoMonitorFolder(string folderPath, int listenPort, string connectionPassword, string serverCertFile, string clientCertFile)
        {
            Console.WriteLine("Starting log monitor on: {0}", folderPath);
            Console.WriteLine("Listening for connections on port {0}", listenPort.ToString());

            PeachFolderPackager packager = new PeachFolderPackager(new FileSystem());

            using (NetworkServerConnection tcpServer = CreateNetworkConnection(listenPort, serverCertFile, clientCertFile))
            {
                FolderMonitor folderMontior = new FolderMonitor(tcpServer, packager, new Clock(), folderPath, connectionPassword);

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
