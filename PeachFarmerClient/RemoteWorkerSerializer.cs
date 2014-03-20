using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class RemoteWorkerSerializer
    {
        public RemoteWorkerSerializer()
        {
            ;
        }

        public void SerializeWorkers(Stream serializationStream, IEnumerable<RemoteWorker> workers)
        {
            foreach (RemoteWorker worker in workers)
            {
                RemoteWorkerToStream(serializationStream, worker);
            }
        }

        public List<RemoteWorker> DeserializeWorkers(RemoteWorkerFactory workerFactory, Stream serializedWorkerInfoList)
        {
            List<RemoteWorker> deserialized = new List<RemoteWorker>();

            while (serializedWorkerInfoList.Position < serializedWorkerInfoList.Length)
            {
                deserialized.Add(RemoteWorkerFromStream(workerFactory, serializedWorkerInfoList));
            }

            return deserialized;
        }

        private RemoteWorker RemoteWorkerFromStream(RemoteWorkerFactory workerFactory, Stream serializedWorker)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string id = ((string)formatter.Deserialize(serializedWorker));
            string remoteAddress = ((string)formatter.Deserialize(serializedWorker));
            DateTime lastPullTime = ((DateTime)formatter.Deserialize(serializedWorker));

            return workerFactory.CreateRemoteWorker(id, remoteAddress, lastPullTime);
        }

        private void RemoteWorkerToStream(Stream serializationStream, RemoteWorker worker)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(serializationStream, worker.Id);
            formatter.Serialize(serializationStream, worker.RemoteAddress);
            formatter.Serialize(serializationStream, worker.LastPullTime);
        }
    }
}
