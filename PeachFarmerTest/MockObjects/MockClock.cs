using RemoteHarvester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    public class MockClock : IClock
    {
        private DateTime _currentTimeUtc;

        public MockClock(DateTime startTime)
        {
            _currentTimeUtc = startTime;
        }

        public DateTime GetCurrentTimeUtc()
        {
            return _currentTimeUtc;
        }

        public void SetCurrentTimeUtc(DateTime currentTimeUtc)
        {
            _currentTimeUtc = currentTimeUtc;
        }
    }
}
