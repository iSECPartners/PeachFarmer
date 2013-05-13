using PeachFarmerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class CommandProcessor
    {
        private List<string> _workerHosts;

        private string _destinationFolder;

        private string _serverPassword;

        private string _serverCertFile;

        private string _clientCertFile;

        public CommandProcessor(CommandLineOptions options)
        {
            _workerHosts = WorkerHostsFromCommandLineOptions(options);

            _destinationFolder = options.DestinationFolder;

            _serverPassword = options.Password;

            _serverCertFile = options.ServerCertFile;

            _clientCertFile = options.ClientCertFile;
        }

        private List<string> WorkerHostsFromCommandLineOptions(CommandLineOptions options)
        {
            List<string> workerHosts = new List<string>();

            if (options.WorkerHost != null)
            {
                workerHosts.Add(options.WorkerHost);
                return workerHosts;
            }

            if (!File.Exists(options.WorkerHostFile))
            {
                Console.WriteLine("Error: File {0} does not exist.", options.WorkerHostFile);
                return null;
            }

            try
            {
                using (Stream workerHostFileStream = File.OpenRead(options.WorkerHostFile))
                {
                    workerHosts.AddRange(TargetFileParser.ParseTargets(workerHostFileStream));
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(string.Format("Error reading file: {0}\r\n{1}", options.WorkerHostFile, ex.Message));
                return null;
            }

            return workerHosts;
        }

        public void PullFilesFromWorkers()
        {
            foreach (string workerHost in _workerHosts)
            {
                try
                {
                    PullFilesFromWorker(workerHost);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine("Connection to {0} failed: {1}", workerHost, ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error pulling files from {0}: {1}", workerHost, ex.Message);
                }
            }
        }

        private void PullFilesFromWorker(string workerHost)
        {
            Console.WriteLine("Pulling files from {0}", workerHost);

            PeachFolderUnpacker unpacker = new PeachFolderUnpacker(new FileSystem(), workerHost);
            PullHistory pullHistory = new PullHistory(new FileSystem(), GetPullHistoryPath(_destinationFolder));

            using (NetworkClientConnection networkConnection = CreateNetworkConnection(workerHost, PeachFarmerProtocol.FarmerPort, _serverCertFile, _clientCertFile))
            {
                FilePuller filePuller = new FilePuller(networkConnection.GetStream(), unpacker, pullHistory, _serverPassword);
                filePuller.Pull(workerHost, _destinationFolder);
            }
        }

        private NetworkClientConnection CreateNetworkConnection(string host, int listenPort, string serverCertFile, string clientCertFile)
        {
            const int ConnectionTimeoutInSeconds = 30;

            if (serverCertFile != null)
            {
                byte[] serverCertData = File.ReadAllBytes(serverCertFile);
                byte[] clientCertData = null;
                if (clientCertFile != null)
                {
                    clientCertData = File.ReadAllBytes(clientCertFile);
                }
                return new NetworkSslClientConnection(host, listenPort, ConnectionTimeoutInSeconds, serverCertData, clientCertData);
            }
            else
            {
                return new NetworkClientConnection(host, listenPort, ConnectionTimeoutInSeconds);
            }
        }

        private string GetPullHistoryPath(string destinationFolder)
        {
            return Path.Combine(destinationFolder, ".peachfarmerhistory.dat");
        }
    }
}
