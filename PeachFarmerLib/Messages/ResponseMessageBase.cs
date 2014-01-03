using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public abstract class ResponseMessageBase : IFarmerNetworkMessage
    {
        public bool IsPasswordCorrect { get; set; }

        public ResponseMessageBase()
        {
            IsPasswordCorrect = false;
        }

        public abstract byte MessageType { get; }

        public bool Equals(ResponseMessageBase other)
        {
            if (this.IsPasswordCorrect != other.IsPasswordCorrect)
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
