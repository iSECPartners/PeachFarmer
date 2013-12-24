using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzingWorker
{
    public class PeachLauncher : IPeachLauncherService
    {
        private string _peachBinPath;
        private string _cmdLineArgs;
        private string _workingDirectory;

        private Process _peachProcess;

        //public PeachLauncher(string peachBinPath, string cmdLineArgs, string workingDirectory)
        //{
        //    _peachBinPath = peachBinPath;
        //    _cmdLineArgs = cmdLineArgs;
        //    _workingDirectory = workingDirectory;

        //    _peachProcess = null;
        //}

        public void Launch(uint startIteration, uint endIteration)
        {
            _peachBinPath = @"D:\peach-3.1.54\Peach.exe";
            _cmdLineArgs = "--seed=0 --definedvalues=rtsp.conf.xml rtsp.xml Vlc";
            _workingDirectory = @"D:\RtspFuzzer";

            string cmdLineArgs = string.Format("--range={0},{1} {2}", startIteration, endIteration, _cmdLineArgs);

            ProcessStartInfo startInfo = new ProcessStartInfo(_peachBinPath, cmdLineArgs);
            startInfo.WorkingDirectory = _workingDirectory;
            startInfo.CreateNoWindow = true;

            _peachProcess = Process.Start(startInfo);
        }

        public void Kill()
        {
            _peachProcess.Kill();
        }
    }
}
