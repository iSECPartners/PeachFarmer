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
    public class ReadRequestMessage : IFarmerNetworkMessage
    {
        public DateTime LastCheckTimeUtc { get; set; }

        public string ServerPassword { get; set; }

        public ReadRequestMessage()
        {
            LastCheckTimeUtc = new DateTime(0);
            ServerPassword = null;
        }

        public byte MessageType
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

            if (this.ServerPassword != other.ServerPassword)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
