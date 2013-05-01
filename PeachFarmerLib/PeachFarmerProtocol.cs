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
        // Server Response  Codes
        //

        public const byte PasswordIncorrect     = 0x10;
        public const byte PasswordCorrect       = 0x11;

        public const int FarmerPort = 8123;
    }
}
