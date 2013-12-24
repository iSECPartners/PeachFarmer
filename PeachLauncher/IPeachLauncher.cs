using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FuzzingWorker
{
    [ServiceContract]
    public interface IPeachLauncherService
    {
        [OperationContract]
        void Launch(uint startIteration, uint endIteration);

        [OperationContract]
        void Kill();
    }
}
