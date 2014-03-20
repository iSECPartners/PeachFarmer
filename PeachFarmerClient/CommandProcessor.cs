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

        private RemoteWorkerFactory _workerFactory;

        private string _destinationFolder;

        private string _serverPassword;

        private SavedWorkerManager _savedWorkerManager;

        public CommandProcessor(CommandLineOptions options)
        {
            IConnectionManager connectionManager = CreateConnectionManager(options.ServerCertFile, options.ClientCertFile);

            _workerFactory = new RemoteWorkerFactory(connectionManager, options.Password);

            _destinationFolder = options.DestinationFolder;

            _serverPassword = options.Password;

            string workerSaveFile = GetSerializedWorkerPath(_destinationFolder);

            _savedWorkerManager = new SavedWorkerManager(workerSaveFile);

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

            _savedWorkerManager.SaveWorkers(_workers);
        }

        private List<RemoteWorker> WorkersFromCommandLineOptions(CommandLineOptions options)
        {
            List<RemoteWorker> remoteWorkers = new List<RemoteWorker>();

            if (options.WorkerHost != null)
            {
                remoteWorkers.Add(_workerFactory.CreateRemoteWorker(options.WorkerHost));
                return remoteWorkers;
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
                            RemoteWorker worker = _workerFactory.CreateRemoteWorker(workerAddress);
                            remoteWorkers.Add(worker);
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
                
                List<Amazon.EC2.Model.Instance> awsWorkers = aws.GetWorkerInstances();
                Console.WriteLine("Found {0} AWS workers", awsWorkers.Count);
                foreach (Amazon.EC2.Model.Instance instance in awsWorkers)
                {
                    RemoteWorker awsWorker = _workerFactory.CreateRemoteWorker(instance.PublicDnsName, instance.InstanceId, null);
                    remoteWorkers.Add(awsWorker);
                }
            }

            remoteWorkers = _savedWorkerManager.UpdateWorkersWithSavedData(_workerFactory, remoteWorkers);

            return remoteWorkers;
        }

        private string GetSerializedWorkerPath(string destinationFolder)
        {
            return Path.Combine(destinationFolder, ".serializedworkers.dat");
        }

        private IConnectionManager CreateConnectionManager(string serverCertFile, string clientCertFile)
        {
            if (string.IsNullOrEmpty(serverCertFile))
            {
                return new ConnectionManager();
            }

            byte[] serverCertData = File.ReadAllBytes(serverCertFile);
            byte[] clientCertData = null;
            if (!string.IsNullOrEmpty(clientCertFile))
            {
                clientCertData = File.ReadAllBytes(clientCertFile);
            }

            return new ConnectionManager(serverCertData, clientCertData);
        }
    }
}
