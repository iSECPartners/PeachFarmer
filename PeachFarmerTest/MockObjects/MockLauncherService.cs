using PeachLauncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvesterTest.MockObjects
{
    class MockLauncherService : ILauncherService
    {
        public UInt64? AssignedStartIteration { get; private set; }

        public UInt64? AssignedEndIteration { get; private set; }

        public bool Killed { get; private set; }

        public MockLauncherService()
        {
            AssignedStartIteration = null;
            AssignedEndIteration = null;
            Killed = false;
        }

        public void Launch(UInt64 startIteration, UInt64 endIteration)
        {
            AssignedStartIteration = startIteration;
            AssignedEndIteration = endIteration;
        }

        public void Kill()
        {
            Killed = true;
        }
    }
}
