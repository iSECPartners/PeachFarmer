using PeachFarmerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class Program
    {
        private const int ConnectionTimeoutInSeconds = 30;

        static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.WorkerHost == null && options.WorkerHostFile == null)
                {
                    Console.WriteLine("Must enter a target host or file of target hosts.");
                    return;
                }

                List<string> workerHosts = WorkerHostsFromCommandLineOptions(options);
                if (workerHosts == null)
                {
                    return;
                }

                PullFilesFromMultipleWorkers(workerHosts, options.DestinationFolder, options.Password, options.ServerCertFile, options.ClientCertFile);
            }
        }

        private static List<string> WorkerHostsFromCommandLineOptions(CommandLineOptions options)
        {
            List<string> workerHosts = new List<string>();

            if (options.WorkerHost != null)
            {
                workerHosts.Add(options.WorkerHost);
                return workerHosts;
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

        private static void PullFilesFromMultipleWorkers(List<string> workerHosts, string destinationFolder, string password, string serverCertFile, string clientCertFile)
        {
            foreach (string workerHost in workerHosts)
            {
                try
                {
                    PullFilesFromWorker(workerHost, destinationFolder, password, serverCertFile, clientCertFile);
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

        private static void PullFilesFromWorker(string workerHost, string destinationFolder, string password, string serverCertFile, string clientCertFile)
        {
            Console.WriteLine("Pulling files from {0}", workerHost);

            PeachFolderUnpacker unpacker = new PeachFolderUnpacker(new FileSystem(), workerHost);
            PullHistory pullHistory = GetPullHistory(destinationFolder);

            using (NetworkClientConnection networkConnection = CreateNetworkConnection(workerHost, PeachFarmerProtocol.FarmerPort, serverCertFile, clientCertFile))
            {
                using (DataConnection dataConnection = new DataConnection(networkConnection))
                {
                    FilePuller filePuller = new FilePuller(dataConnection, unpacker, pullHistory, password);
                    filePuller.Pull(workerHost, destinationFolder);
                }
            }

            SavePullHistory(pullHistory, destinationFolder);
        }

        private static NetworkClientConnection CreateNetworkConnection(string host, int listenPort, string serverCertFile, string clientCertFile)
        {
            if (serverCertFile != null)
            {
                return new NetworkSslClientConnection(host, listenPort, ConnectionTimeoutInSeconds, serverCertFile, clientCertFile);
            }
            else
            {
                return new NetworkClientConnection(host, listenPort, ConnectionTimeoutInSeconds);
            }
        }

        private static string GetPullHistoryPath(string destinationFolder)
        {
            return Path.Combine(destinationFolder, ".peachfarmerhistory.dat");
        }

        private static PullHistory GetPullHistory(string destinationFolder)
        {
            PullHistory pullHistory;

            string filePath = GetPullHistoryPath(destinationFolder);
            if (!File.Exists(filePath))
            {
                return new PullHistory();
            }

            using (Stream pullHistoryStream = File.OpenRead(filePath))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                pullHistory = (PullHistory)deserializer.Deserialize(pullHistoryStream);
            }

            return pullHistory;
        }

        private static void SavePullHistory(PullHistory pullHistory, string destinationFolder)
        {
            string filePath = GetPullHistoryPath(destinationFolder);
            using (Stream pullHistoryStream = File.OpenWrite(filePath))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(pullHistoryStream, pullHistory);
            }
        }
    }
}
