using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public enum WorkerSyncActions
    {
        RetrieveFiles,
    }

    public class WorkerManager
    {
        // WorkerStatus
        //  Last iteration
        //  Total iterations
        //

        public void SyncWithWorkers(WorkerSyncActions actions)
        {

        }
        public void SyncWithWorker(IWorkerInfo workerInfo, WorkerSyncActions actions)
        {

        }
    }
}
