using CommandLine;
using CommandLine.Text;
using PeachFarmerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class Program
    {
        private const int ConnectionTimeout = 5;

        static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Password != null && options.ServerCertFile == null)
                {
                    ShowPlaintextPasswordWarning();
                }

                DoMonitorFolder(options.LogFolder, PeachFarmerProtocol.FarmerPort, options.Password, options.ServerCertFile, options.ClientCertFile);
            }
        }

        private static void ShowPlaintextPasswordWarning()
        {
            Console.WriteLine("Warning: Server password is required but SSL is not enabled. Passwords will be sent in plaintext.\r\n" +
                              "Use the --server-cert parameter to enable SSL so that passwords are sent in an encrypted SSL tunnel.");
            Console.WriteLine();
        }

        private static void DoMonitorFolder(string folderPath, int listenPort, string connectionPassword, string serverCertFile, string clientCertFile)
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

        private static NetworkServerConnection CreateNetworkConnection(int listenPort, string serverCertFile, string clientCertFile)
        {
            if (serverCertFile != null)
            {
                return new NetworkSslServerConnection(listenPort, ConnectionTimeout, serverCertFile, clientCertFile);
            }
            else
            {
                return new NetworkServerConnection(listenPort, ConnectionTimeout);
            }
        }
    }
}
