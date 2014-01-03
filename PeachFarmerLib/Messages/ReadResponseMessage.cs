using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public class ReadResponseMessage : ResponseMessageBase
    {
        public DateTime CurrentServerTimeUtc { get; set; }

        public byte[] Data { get; set; }

        public ReadResponseMessage()
        {
            CurrentServerTimeUtc = new DateTime();
            Data = null;
        }

        public override byte MessageType
        {
            get { return PeachFarmerProtocol.ReadResponse; }
        }
    }
}
