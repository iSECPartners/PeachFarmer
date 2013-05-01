using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface INetworkConnection : IDisposable
    {
        byte[] ReadBytes(int length);

        void WriteBytes(byte[] data);

        void Close();
    }
}
