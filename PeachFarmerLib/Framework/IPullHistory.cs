using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface IPullHistory
    {
        DateTime GetLastPullTime(string workerHost);

        void SetLastPullTime(string workerHost, DateTime utcTime);

        void Save();

        void Load();
    }
}
