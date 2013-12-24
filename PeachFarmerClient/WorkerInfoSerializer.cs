using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class WorkerInfoSerializer
    {
        public void SerializeWorkerInfoList(Stream serializationStream, List<IWorkerInfo> workerInfoList)
        {
            foreach (IWorkerInfo workerInfo in workerInfoList)
            {
                WorkerInfoToStream(serializationStream, workerInfo);
            }
        }

        public List<IWorkerInfo> DeserializeWorkerInfoList(Stream serializedWorkerInfoList)
        {
            List<IWorkerInfo> deserialized = new List<IWorkerInfo>();

            while (serializedWorkerInfoList.Position < serializedWorkerInfoList.Length)
            {
                deserialized.Add(WorkerInfoFromStream(serializedWorkerInfoList));
            }

            return deserialized;
        }

        private IWorkerInfo WorkerInfoFromStream(Stream serializationStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            return ((IWorkerInfo)formatter.Deserialize(serializationStream));
        }

        private void WorkerInfoToStream(Stream serializationStream, IWorkerInfo workerInfo)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(serializationStream, workerInfo);
        }
    }
}
