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
    public class RemoteWorkerFactory
    {
        private IConnectionManager _connectionManager;

        private string _serverPassword;

        public RemoteWorkerFactory(IConnectionManager connectionManager, string serverPassword)
        {
            _connectionManager = connectionManager;

            _serverPassword = serverPassword;
        }

        public RemoteWorker CreateRemoteWorker(string workerAddress)
        {
            return CreateRemoteWorker(workerAddress, null, null);
        }

        public RemoteWorker CreateRemoteWorker(string workerAddress, string workerId, DateTime? lastPullTime)
        {
            FileSystem fs = new FileSystem();

            string unpackerDisambiguator;
            if (workerId != null)
            {
                unpackerDisambiguator = workerId;
            }
            else
            {
                unpackerDisambiguator = workerAddress;
            }

            FolderUnpacker unpacker = new FolderUnpacker(fs, unpackerDisambiguator);

            NetworkClientConnection networkConnection = _connectionManager.CreateNetworkConnection(workerAddress, PeachFarmerProtocol.FarmerPort);

            DateTime pullTime = DateTime.MinValue;
            if (lastPullTime.HasValue)
            {
                pullTime = (DateTime)lastPullTime;
            }

            FilePuller filePuller = new FilePuller(unpacker, _serverPassword, pullTime);

            return new RemoteWorker(workerId, networkConnection, filePuller, new StatusFileParser());
        }
    }
}
