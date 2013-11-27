using PeachFarmerClient.Framework;
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

            SaveWorkerInfo();
        }

        private List<RemoteWorker> WorkersFromCommandLineOptions(CommandLineOptions options)
        {
            List<IWorkerInfo> workersInfo = WorkerInfoFromCommandLineOptions(options);

            workersInfo = UpdateWorkerInfoFromSaveFile(workersInfo);

            return WorkersFromInfo(workersInfo);
        }

        private List<IWorkerInfo> UpdateWorkerInfoFromSaveFile(List<IWorkerInfo> workersInfo)
        {
            List<IWorkerInfo> savedWorkersInfo = WorkerInfoFromSaveFile(_destinationFolder);

            if (savedWorkersInfo == null)
            {
                return workersInfo;
            }

            List<IWorkerInfo> updatedWorkersInfo = new List<IWorkerInfo>();

            foreach (IWorkerInfo commandLineWorkerInfo in workersInfo)
            {
                IWorkerInfo matchingInfo = FindMatchingWorkerInfo(savedWorkersInfo, commandLineWorkerInfo);

                if (matchingInfo != null)
                {
                    updatedWorkersInfo.Add(matchingInfo);
                }
                else
                {
                    updatedWorkersInfo.Add(commandLineWorkerInfo);
                }
            }

            return updatedWorkersInfo;
        }

        private IWorkerInfo FindMatchingWorkerInfo(IEnumerable<IWorkerInfo> workersInfo, IWorkerInfo toMatch)
        {
            foreach (IWorkerInfo workerInfo in workersInfo)
            {
                if ((workerInfo.Id == toMatch.Id) || (workerInfo.RemoteAddress == toMatch.RemoteAddress))
                {
                    return workerInfo;
                }
            }

            return null;
        }

        private List<IWorkerInfo> WorkerInfoFromSaveFile(string _destinationFolder)
        {
            List<IWorkerInfo> savedWorkersInfo = new List<IWorkerInfo>();
            string savedFilePath = GetWorkerInfoPath(_destinationFolder);

            if (!File.Exists(savedFilePath))
            {
                return null;
            }

            try
            {
                using (Stream savedInfoStream = File.OpenRead(savedFilePath))
                {
                    while (savedInfoStream.Position < savedInfoStream.Length)
                    {
                        savedWorkersInfo.Add(WorkerInfoFromStream(savedInfoStream));
                    }
                }
            }
            catch (IOException)
            {
                return null;
            }

            return savedWorkersInfo;
        }

        private void SaveWorkerInfo()
        {
            List<IWorkerInfo> savedWorkersInfo = new List<IWorkerInfo>();
            string savedFilePath = GetWorkerInfoPath(_destinationFolder);

            using (Stream savedInfoStream = File.OpenWrite(savedFilePath))
            {
                foreach (RemoteWorker worker in _workers)
                {
                    WorkerInfoToStream(savedInfoStream, worker.GetInfo());
                }
            }
        }

        private IWorkerInfo WorkerInfoFromStream(Stream serializationStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            return ((IWorkerInfo) formatter.Deserialize(serializationStream));
        }

        private void WorkerInfoToStream(Stream serializationStream, IWorkerInfo workerInfo)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(serializationStream, workerInfo);
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

            foreach (IWorkerInfo workerInfo in workersInfo)
            {
                workers.Add(WorkerFromInfo(workerInfo));
            }

            return workers;
        }

        private RemoteWorker WorkerFromInfo(IWorkerInfo workerInfo)
        {
            FileSystem fs = new FileSystem();

            string unpackerDisambiguator;
            if (workerInfo.Id != null)
            {
                unpackerDisambiguator = workerInfo.Id;
            }
            else
            {
                unpackerDisambiguator = workerInfo.RemoteAddress;
            }

            FolderUnpacker unpacker = new FolderUnpacker(fs, unpackerDisambiguator);

            NetworkClientConnection networkConnection = CreateNetworkConnection(workerInfo.RemoteAddress,
                                                                                PeachFarmerProtocol.FarmerPort,
                                                                                _serverCertFile,
                                                                                _clientCertFile);
            FilePuller filePuller = new FilePuller(unpacker, _serverPassword, workerInfo.LastPullTime);

            return new RemoteWorker(workerInfo.Id, networkConnection, filePuller, new StatusFileParser());
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

        private string GetWorkerInfoPath(string destinationFolder)
        {
            return Path.Combine(destinationFolder, ".workerinfo.dat");
        }
    }
}
