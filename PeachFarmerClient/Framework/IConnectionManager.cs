using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient.Framework
{
    public interface IConnectionManager
    {
        NetworkClientConnection CreateNetworkConnection(string host, int listenPort);
    }
}
