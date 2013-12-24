using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient.Framework
{
    public interface INetworkClientConnection : INetworkConnection
    {
        string RemoteAddress { get; }

        int RemotePort { get; }
    }
}
