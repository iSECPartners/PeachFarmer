using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    [Serializable]
    public class WorkerInfo : IWorkerInfo
    {
        public string Id { get; private set;}

        public string RemoteAddress { get; private set; }

        public DateTime LastPullTime { get; private set; }
        
        public WorkerInfo(string remoteAddress)
            :this(null, remoteAddress)
        {
        }

        public WorkerInfo(string id, string remoteAddress)
            : this(id, remoteAddress, DateTime.MinValue)
        {
        }

        public WorkerInfo(string id, string remoteAddress, DateTime lastPullTime)
        {
            Id = id;
            RemoteAddress = remoteAddress;
            LastPullTime = lastPullTime;
        }
    }
}
