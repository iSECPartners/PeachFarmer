using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public interface IClock
    {
        DateTime GetCurrentTimeUtc();
    }
}
