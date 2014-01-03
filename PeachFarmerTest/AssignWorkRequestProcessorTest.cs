using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteHarvester;
using RemoteHarvesterTest.MockObjects;
using PeachFarmerLib.Messages;

namespace RemoteHarvesterTest
{
    [TestClass]
    public class AssignWorkRequestProcessorTest
    {
        private AssignWorkResponseMessage ProcessAssignWorkRequest(AssignWorkRequestProcessor processor, UInt64 startIteration, UInt64 endIteration, string password)
        {
            AssignWorkRequestMessage request = new AssignWorkRequestMessage(startIteration, endIteration, password);
            return (AssignWorkResponseMessage)processor.Process(request);
        }

        [TestMethod]
        public void ProcessTest()
        {
            UInt64 startIteration = 50;
            UInt64 endIteration = 75;
            string password = null;
            MockLauncherService mockLauncher = new MockLauncherService();
            AssignWorkRequestProcessor processor = new AssignWorkRequestProcessor(mockLauncher, password);
            AssignWorkResponseMessage response = ProcessAssignWorkRequest(processor, startIteration, endIteration, password);

            Assert.IsTrue(response.IsPasswordCorrect);
            Assert.AreEqual(AssignWorkResponseMessage.AssignmentResult.Succeeded, response.Result);
            Assert.AreEqual(startIteration, mockLauncher.AssignedStartIteration);
            Assert.AreEqual(endIteration, mockLauncher.AssignedEndIteration);
            Assert.IsFalse(mockLauncher.Killed);
        }

        [TestMethod]
        public void ProcessWrongPasswordTest()
        {
            string correctPassword = "c0rr3ct_p4ssword!";
            string suppliedPassword = "cheese";
            MockLauncherService mockLauncher = new MockLauncherService();
            AssignWorkRequestProcessor processor = new AssignWorkRequestProcessor(mockLauncher, correctPassword);
            AssignWorkResponseMessage response = ProcessAssignWorkRequest(processor, 50, 75, suppliedPassword);

            Assert.IsFalse(response.IsPasswordCorrect);
            Assert.AreEqual(AssignWorkResponseMessage.AssignmentResult.Unknown, response.Result);
            Assert.IsNull(mockLauncher.AssignedStartIteration);
            Assert.IsNull(mockLauncher.AssignedEndIteration);
            Assert.IsFalse(mockLauncher.Killed);
        }
    }
}
