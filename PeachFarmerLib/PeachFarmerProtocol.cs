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
        public const byte AssignWorkRequest = 0x0b;

        //
        // Server Message Codes
        //

        public const byte ReadResponse = 0x8a;
        public const byte AssignWorkResponse = 0x8b;

        public const int FarmerPort = 8123;
    }
}
