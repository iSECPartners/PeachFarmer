using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachLauncher
{
    public class Launcher : ILauncherService
    {
        private Process _peachProcess;

        public string PeachBinaryPath { get; set; }
        
        public string PeachCmdLineArgs { get; set; }
        
        public string PeachWorkingDirectory { get; set; }

        public void Launch(UInt64 startIteration, UInt64 endIteration)
        {
            string cmdLineArgs = string.Format("--range={0},{1} {2}", startIteration, endIteration, PeachCmdLineArgs);

            ProcessStartInfo startInfo = new ProcessStartInfo(PeachBinaryPath, cmdLineArgs);
            startInfo.WorkingDirectory = PeachWorkingDirectory;
            startInfo.CreateNoWindow = true;

            _peachProcess = Process.Start(startInfo);
        }

        public void Kill()
        {
            _peachProcess.Kill();
        }
    }
}
