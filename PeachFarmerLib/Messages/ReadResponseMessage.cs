using PeachFarmerLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Messages
{
    [Serializable()]
    public class ReadResponseMessage : IFarmerNetworkMessage
    {
        public DateTime CurrentServerTimeUtc { get; set; }

        public bool IsPasswordCorrect { get; set; }

        public byte[] Data { get; set; }

        public ReadResponseMessage()
        {
            CurrentServerTimeUtc = new DateTime();
            IsPasswordCorrect = false;
            Data = null;
        }

        public byte MessageType
        {
            get { return PeachFarmerProtocol.ReadResponse; }
        }
    }
}
