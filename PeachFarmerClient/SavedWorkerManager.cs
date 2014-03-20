using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class SavedWorkerManager
    {
        private string _workerSaveFile;

        private RemoteWorkerSerializer _workerSerializer;

        public SavedWorkerManager(string workerSaveFile)
        {
            _workerSaveFile = workerSaveFile;

            _workerSerializer = new RemoteWorkerSerializer();
        }

        public void SaveWorkers(IEnumerable<RemoteWorker> workers)
        {
            using (Stream savedWorkerStream = File.OpenWrite(_workerSaveFile))
            {
                _workerSerializer.SerializeWorkers(savedWorkerStream, workers);
            }
        }

        public List<RemoteWorker> UpdateWorkersWithSavedData(RemoteWorkerFactory workerFactory, List<RemoteWorker> workers)
        {
            List<RemoteWorker> deserializedWorkers = LoadWorkers(workerFactory);

            //
            // If no workers were saved, there is no information to update, so just use the original
            // list of workers.
            //

            if (deserializedWorkers == null)
            {
                return workers;
            }

            List<RemoteWorker> updatedWorkersInfo = new List<RemoteWorker>();

            foreach (RemoteWorker worker in workers)
            {
                RemoteWorker updatedWorker = FindMatchingWorker(deserializedWorkers, worker);

                if (updatedWorker != null)
                {
                    updatedWorkersInfo.Add(updatedWorker);
                }
                else
                {
                    updatedWorkersInfo.Add(worker);
                }
            }

            return updatedWorkersInfo;
        }

        private RemoteWorker FindMatchingWorker(IEnumerable<RemoteWorker> workers, RemoteWorker toMatch)
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

        private List<RemoteWorker> LoadWorkers(RemoteWorkerFactory workerFactory)
        {
            List<RemoteWorker> savedWorkers;

            if (!File.Exists(_workerSaveFile))
            {
                return null;
            }

            try
            {
                using (Stream savedInfoStream = File.OpenRead(_workerSaveFile))
                {
                    savedWorkers = _workerSerializer.DeserializeWorkers(workerFactory, savedInfoStream);
                }
            }
            catch (IOException)
            {
                return null;
            }

            return savedWorkers;
        }
    }
}
