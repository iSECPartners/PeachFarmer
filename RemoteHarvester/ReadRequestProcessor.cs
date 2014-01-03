using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class ReadRequestProcessor : RequestProcessorBase
    {
        private IFolderPacker _folderPackager;

        private IClock _clock;

        private string _folderPath;

        public ReadRequestProcessor(IFolderPacker folderPackager, IClock clock, string folderPath, string password)
            :base(password)
        {
            _folderPackager = folderPackager;
            _clock = clock;
            _folderPath = folderPath;
        }

        protected override void ProcessImpl(RequestMessageBase requestMessage, ResponseMessageBase responseMessage)
        {
            ProcessImpl((ReadRequestMessage)requestMessage, (ReadResponseMessage)responseMessage);
        }

        protected void ProcessImpl(ReadRequestMessage requestMessage, ReadResponseMessage responseMessage)
        {
            responseMessage.CurrentServerTimeUtc = _clock.GetCurrentTimeUtc();
            responseMessage.Data = _folderPackager.PackFolder(_folderPath, requestMessage.LastCheckTimeUtc);
        }

        protected override ResponseMessageBase CreateResponseMessage()
        {
            return new ReadResponseMessage();
        }
    }
}
