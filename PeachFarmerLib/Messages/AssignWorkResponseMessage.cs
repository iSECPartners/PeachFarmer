using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public class AssignWorkResponseMessage : ResponseMessageBase
    {
        public enum AssignmentResult
        {
            Unknown,
            Succeeded,
            Error
        }

        public AssignmentResult Result { get; set; }

        public AssignWorkResponseMessage()
        {
            Result = AssignmentResult.Unknown;
        }

        public override byte MessageType
        {
            get { return PeachFarmerProtocol.AssignWorkResponse; }
        }
    }
}
