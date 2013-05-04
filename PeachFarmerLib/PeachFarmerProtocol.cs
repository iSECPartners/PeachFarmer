using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib
{
    public static class PeachFarmerProtocol
    {
        //
        // Client Message Codes
        //

        public const byte ReadRequest = 0x0a;

        //
        // Server Message Codes
        //

        public const byte ReadResponse = 0x8a;

        public const int FarmerPort = 8123;
    }
}
