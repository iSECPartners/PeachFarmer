using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using PeachFarmerClient.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient
{
    public class AwsWorkerController : ICloudWorkerController
    {
        IAmazonEC2 _ec2Client;
        public AwsWorkerController()
        {
            _ec2Client = AWSClientFactory.CreateAmazonEC2Client();
        }

        public List<Instance> GetWorkerInstances()
        {
            List<Instance> instances = new List<Instance>();

            try
            {
                DescribeInstancesResponse ec2Response = _ec2Client.DescribeInstances();
                foreach (Reservation r in ec2Response.Reservations)
                {
                    instances.AddRange(r.Instances);
                }
            }
            catch (AmazonEC2Exception ex)
            {
                if (ex.ErrorCode != null && ex.ErrorCode.Equals("AuthFailure"))
                {
                    Console.WriteLine("The account you are using is not signed up for Amazon EC2.");
                    Console.WriteLine("You can sign up for Amazon EC2 at http://aws.amazon.com/ec2");
                }
                else
                {
                    Console.WriteLine("Caught Exception: " + ex.Message);
                    Console.WriteLine("Response Status Code: " + ex.StatusCode);
                    Console.WriteLine("Error Code: " + ex.ErrorCode);
                    Console.WriteLine("Error Type: " + ex.ErrorType);
                    Console.WriteLine("Request ID: " + ex.RequestId);
                }
            }

            return instances;
        }
    }
}
