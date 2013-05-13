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
            new Program(args);
        }

        private Program(String[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                string usageError;
                if (!options.Validate(out usageError))
                {
                    Console.WriteLine(options.GetUsage());
                    Console.WriteLine("Error: " + usageError);
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

        private List<string> WorkerHostsFromCommandLineOptions(CommandLineOptions options)
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

        private void PullFilesFromMultipleWorkers(List<string> workerHosts, string destinationFolder, string password, string serverCertFile, string clientCertFile)
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

        private void PullFilesFromWorker(string workerHost, string destinationFolder, string password, string serverCertFile, string clientCertFile)
        {
            Console.WriteLine("Pulling files from {0}", workerHost);

            PeachFolderUnpacker unpacker = new PeachFolderUnpacker(new FileSystem(), workerHost);
            PullHistory pullHistory = new PullHistory(new FileSystem(), GetPullHistoryPath(destinationFolder));

            using (NetworkClientConnection networkConnection = CreateNetworkConnection(workerHost, PeachFarmerProtocol.FarmerPort, serverCertFile, clientCertFile))
            {
                FilePuller filePuller = new FilePuller(networkConnection.GetStream(), unpacker, pullHistory, password);
                filePuller.Pull(workerHost, destinationFolder);
            }
        }

        private NetworkClientConnection CreateNetworkConnection(string host, int listenPort, string serverCertFile, string clientCertFile)
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

        private string GetPullHistoryPath(string destinationFolder)
        {
            return Path.Combine(destinationFolder, ".peachfarmerhistory.dat");
        }
    }
}
