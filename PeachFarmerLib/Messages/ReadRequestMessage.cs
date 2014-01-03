using PeachFarmerLib;
using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public class ReadRequestMessage : RequestMessageBase
    {
        public DateTime LastCheckTimeUtc { get; set; }

        public ReadRequestMessage(DateTime lastCheckTimeUtc, string password)
            :base(password)
        {
            LastCheckTimeUtc = lastCheckTimeUtc;
        }

        public override byte MessageType
        {
            get { return PeachFarmerProtocol.ReadRequest; }
        }

        public override bool Equals(object other)
        {
            return Equals((ReadRequestMessage)other);
        }

        public bool Equals(ReadRequestMessage other)
        {
            if (this.LastCheckTimeUtc != other.LastCheckTimeUtc)
            {
                return false;
            }

            return base.Equals(other);
        }
    }
}
