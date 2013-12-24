using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FuzzingWorker
{
    [ServiceContract]
    public interface ILauncherService
    {
        [OperationContract]
        void Launch(UInt64 startIteration, UInt64 endIteration);

        [OperationContract]
        void Kill();
    }
}
