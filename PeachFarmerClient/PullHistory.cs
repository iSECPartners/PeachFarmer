using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    [Serializable()]
    public class PullHistory : IPullHistory
    {
        Dictionary<string, DateTime> _lastPullTimes;

        public PullHistory()
        {
            _lastPullTimes = new Dictionary<string, DateTime>();
        }

        public DateTime GetLastPullTime(string workerHost)
        {
            try
            {
                return _lastPullTimes[workerHost];
            }
            catch (KeyNotFoundException)
            {
                return new DateTime(0);
            }
        }

        public void SetLastPullTime(string workerHost, DateTime utcTime)
        {
            if (_lastPullTimes.ContainsKey(workerHost))
            {
                _lastPullTimes[workerHost] = utcTime;
            }
            else
            {
                _lastPullTimes.Add(workerHost, utcTime);
            }
        }
    }
}
