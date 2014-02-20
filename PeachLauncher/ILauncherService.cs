using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PeachLauncher
{
    [ServiceContract]
    public interface ILauncherService
    {
        string PeachBinaryPath { get; set; }

        string PeachCmdLineArgs { get; set; }

        string PeachWorkingDirectory { get; set; }

        [OperationContract]
        void Launch(UInt64 startIteration, UInt64 endIteration);

        [OperationContract]
        void Kill();
    }
}
