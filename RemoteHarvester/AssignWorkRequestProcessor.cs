using PeachLauncher;
using PeachFarmerLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvester
{
    public class AssignWorkRequestProcessor : RequestProcessorBase
    {
        private ILauncherService _launcherService;

        public AssignWorkRequestProcessor(ILauncherService launcherService, string password)
            :base(password)
        {
            _launcherService = launcherService;
        }

        protected override void ProcessImpl(RequestMessageBase request, ResponseMessageBase response)
        {
            ProcessImpl((AssignWorkRequestMessage)request, (AssignWorkResponseMessage)response);
        }

        protected void ProcessImpl(AssignWorkRequestMessage request, AssignWorkResponseMessage response)
        {
            _launcherService.Launch(request.StartIteration, request.EndIteration);

            response.Result = AssignWorkResponseMessage.AssignmentResult.Succeeded;
        }

        protected override ResponseMessageBase CreateResponseMessage()
        {
            return new AssignWorkResponseMessage();
        }
    }
}
