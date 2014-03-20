using PeachFarmerClient.Framework;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class RemoteWorker
    {
        private string _id;

        private INetworkClientConnection _networkConnection;

        private IFilePuller _filePuller;

        private IStatusFileParser _statusFileParser;

        // PeachJob

        //      LastCompletedIteration

        //      StartIteration

        //      EndIteration

        public RemoteWorker(INetworkClientConnection networkConnection, IFilePuller filePuller, IStatusFileParser statusFileParser)
            : this(null, networkConnection, filePuller, statusFileParser)
        {
            ;
        }

        public RemoteWorker(string id, INetworkClientConnection networkConnection, IFilePuller filePuller, IStatusFileParser statusFileParser)
        {
            _id = id;
            _networkConnection = networkConnection;
            _filePuller = filePuller;
            _statusFileParser = statusFileParser;
        }

        public string Id
        {
            get
            {
                if (_id != null)
                {
                    return _id;
                }
                else
                {
                    return _networkConnection.RemoteAddress;
                }
            }
        }

        public string RemoteAddress
        {
            get
            {
                return _networkConnection.RemoteAddress;
            }
        }

        public DateTime LastPullTime
        {
            get
            {
                return _filePuller.LastPullTime;
            }
        }

        public void PullFiles(string destinationFolder)
        {
            Console.WriteLine("Pulling files from {0}", this.ToString());

            using (Stream workerStream = _networkConnection.GetStream())
            {
                _filePuller.Pull(workerStream, destinationFolder);
            }

            PrintLastStatus();
        }

        private void PrintLastStatus()
        {
            using (Stream statusFileStream = _filePuller.GetStatusFileStream())
            {
                if (statusFileStream == null)
                {
                    return;
                }

                PeachStatus status = _statusFileParser.Parse(statusFileStream);
                if (status.LastCompletedIteration != null)
                {
                    Console.WriteLine("Last iteration: {0}", status.LastCompletedIteration);
                }
                if (status.LastUpdate != null)
                {
                    Console.WriteLine("Last activity: {0}", status.LastUpdate);
                }
                if (status.Finished)
                {
                    Console.WriteLine("Worker no longer running");
                }
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_id))
            {
                return string.Format("{0} ({1})", _id, _networkConnection.RemoteAddress);
            }
            else
            {
                return _networkConnection.RemoteAddress;
            }
        }

        public static List<RemoteWorker> MergeWorkers(List<RemoteWorker> primaryList, List<RemoteWorker> extraList)
        {
            List<RemoteWorker> mergedList = new List<RemoteWorker>();

            foreach (RemoteWorker primaryWorker in primaryList)
            {
                RemoteWorker matchingWorker = FindMatchingWorker(extraList, primaryWorker);

                if (matchingWorker != null)
                {
                    mergedList.Add(matchingWorker);
                }
                else
                {
                    mergedList.Add(primaryWorker);
                }
            }

            return mergedList;
        }

        private static RemoteWorker FindMatchingWorker(IEnumerable<RemoteWorker> workers, RemoteWorker toMatch)
        {
            foreach (RemoteWorker worker in workers)
            {
                if ((!string.IsNullOrEmpty(worker.Id) && (worker.Id == toMatch.Id)) ||
                    (worker.RemoteAddress == toMatch.RemoteAddress))
                {
                    return worker;
                }
            }

            return null;
        }

        /*
        public void AssignJob(UInt64 startIteration, UInt64 endIteration)
        {

        }

        public bool JobComplete()
        {
            return false;
        }*/

    }
}
