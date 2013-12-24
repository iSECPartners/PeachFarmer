using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class AwsWorkerInfo : WorkerInfo
    {
        public override WorkerService Service
        {
            get { return WorkerService.Aws; }
        }

        public AwsWorkerInfo(string assetId, string remoteAddress)
            :base(assetId, remoteAddress)
        {
            ;
        }
    }
}
