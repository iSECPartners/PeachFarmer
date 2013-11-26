using PeachFarmerClient.Framework;
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
        private List<RemoteWorker> _workers;

        private string _destinationFolder;

        private string _serverPassword;

        private string _serverCertFile;

        private string _clientCertFile;

        public CommandProcessor(CommandLineOptions options)
        {
            _destinationFolder = options.DestinationFolder;

            _serverPassword = options.Password;

            _serverCertFile = options.ServerCertFile;

            _clientCertFile = options.ClientCertFile;

            _workers = WorkersFromCommandLineOptions(options);
        }

        public void PullFilesFromWorkers()
        {
            foreach (RemoteWorker worker in _workers)
            {
                try
                {
                    worker.PullFiles(_destinationFolder);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    Console.WriteLine("Connection to {0} failed: {1}", worker, ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error pulling files from {0}: {1}", worker, ex.Message);
                }
            }
        }

        private List<RemoteWorker> WorkersFromCommandLineOptions(CommandLineOptions options)
        {
            List<IWorkerInfo> workersInfo = WorkerInfoFromCommandLineOptions(options);

            return WorkersFromInfo(workersInfo);
        }

        private List<IWorkerInfo> WorkerInfoFromCommandLineOptions(CommandLineOptions options)
        {
            List<IWorkerInfo> workerHosts = new List<IWorkerInfo>();

            if (options.WorkerHost != null)
            {
                workerHosts.Add(new WorkerInfo(options.WorkerHost));
                return workerHosts;
            }

            if (options.WorkerHostFile != null)
            {
                if (!File.Exists(options.WorkerHostFile))
                {
                    Console.WriteLine("Error: File {0} does not exist.", options.WorkerHostFile);
                    return null;
                }
                try
                {
                    using (Stream workerHostFileStream = File.OpenRead(options.WorkerHostFile))
                    {
                        foreach (string workerAddress in TargetFileParser.ParseTargets(workerHostFileStream))
                        {
                            IWorkerInfo workerInfo = new WorkerInfo(workerAddress);
                            workerHosts.Add(workerInfo);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(string.Format("Error reading file: {0}\r\n{1}", options.WorkerHostFile, ex.Message));
                    return null;
                }
            }

            if (options.UseAws)
            {
                AwsWorkerController aws = new AwsWorkerController();
                List<IWorkerInfo> workers = aws.GetWorkersInfo();
                Console.WriteLine("Found {0} AWS workers", workers.Count);
                foreach (IWorkerInfo worker in workers)
                {
                    Console.WriteLine(worker);
                }
            }

            return workerHosts;
        }

        private List<RemoteWorker> WorkersFromInfo(IEnumerable<IWorkerInfo> workersInfo)
        {
            List<RemoteWorker> workers = new List<RemoteWorker>();

            FileSystem fs = new FileSystem();
            PullHistory pullHistory = new PullHistory(new FileSystem(), GetPullHistoryPath(_destinationFolder));
            StatusFileParser statusFileParser = new StatusFileParser();

            foreach (IWorkerInfo workerInfo in workersInfo)
            {
                FolderUnpacker unpacker = new FolderUnpacker(new FileSystem(), workerInfo.Id);

                NetworkClientConnection networkConnection = CreateNetworkConnection(workerInfo.RemoteAddress,
                                                                                    PeachFarmerProtocol.FarmerPort,
                                                                                    _serverCertFile,
                                                                                    _clientCertFile);
                FilePuller filePuller = new FilePuller(unpacker, pullHistory.GetLastPullTime(workerInfo.Id), _serverPassword);

                RemoteWorker worker = new RemoteWorker(workerInfo.Id, networkConnection, filePuller, statusFileParser);
                workers.Add(worker);
            }

            return workers;
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
