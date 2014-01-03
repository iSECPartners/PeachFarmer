using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    public class AssignWorkRequestMessage : RequestMessageBase
    {
        public UInt64 StartIteration { get; private set; }

        public UInt64 EndIteration { get; private set; }

        public AssignWorkRequestMessage(UInt64 startIteration, UInt64 endIteration, string password)
            :base(password)
        {
            StartIteration = startIteration;
            EndIteration = endIteration;
        }

        public override byte MessageType
        {
            get { return PeachFarmerProtocol.AssignWorkRequest; }
        }

        public override bool Equals(object other)
        {
            return Equals((AssignWorkRequestMessage)other);
        }

        public bool Equals(AssignWorkRequestMessage other)
        {
            if (this.StartIteration != other.StartIteration)
            {
                return false;
            }

            if (this.EndIteration != other.EndIteration)
            {
                return false;
            }

            return base.Equals(other);
        }
    }
}
