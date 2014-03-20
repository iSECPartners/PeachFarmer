using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient.Framework
{
    public interface ICloudWorkerController
    {
        List<Instance> GetWorkerInstances();
    }
}
