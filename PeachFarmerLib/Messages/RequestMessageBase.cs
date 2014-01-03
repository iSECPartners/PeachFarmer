using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public abstract class RequestMessageBase : IFarmerNetworkMessage
    {
        public string ServerPassword { get; set; }

        /*public RequestMessageBase()
            :this(null)
        {
        }*/

        public RequestMessageBase(string password)
        {
            ServerPassword = password;
        }

        public abstract byte MessageType { get; }

        public bool Equals(RequestMessageBase other)
        {
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
