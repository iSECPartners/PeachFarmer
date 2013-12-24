using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace FuzzingWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            // PeachFarmerClient polls every 60 seconds
            //
            // Requests status
            //      If harvester is running
            //          If timeout elapsed
            //              give new job
            //      Else
            //          give new job
            //      
            //
            // messages:
            //   start job   - kills any existing process, starts job
            //   
            //

            Uri baseAddress = new Uri("http://localhost:8080/hello");

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(Launcher), baseAddress))
            {
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }
        }
    }
}
