using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteHarvester;
using System.Collections;
using PeachFarmerLib;
using System.IO;
using System.Collections.Generic;
using PeachFarmerTest.MockObjects;
using PeachFarmerTest;

namespace RemoteHarvesterTest
{
    [TestClass]
    public class FolderMonitorTest
    {
        const string CorrectPassword = "the_r1ght_passw0rd";

        private void VerifyMonitoring(DateTime startTimestampExpected, byte[] folderDataExpected)
        {
            MockDataConnection mockDataConnection = new MockDataConnection();
            MockFolderPackager mockFolderPackager = new MockFolderPackager();
            MockClock serverClock = new MockClock(new DateTime(2007, 5, 25, 15, 27, 03));

            //
            // All that the server expects from the client is a read request byte and a minimum
            // modification timestamp.
            //

            mockDataConnection.BytesToReceiveQueue.Add(PeachFarmerProtocol.ReadRequest);
            mockDataConnection.DateTimesToReceiveQueue.Add(startTimestampExpected);
            mockDataConnection.StringsToReceiveQueue.Add(CorrectPassword);

            FolderMonitor folderMonitor = new FolderMonitor(mockDataConnection, mockFolderPackager, serverClock, @"c:\fakepath\fakedir", CorrectPassword);
            folderMonitor.ProcessNextRequest();
            Assert.AreEqual(@"c:\fakepath\fakedir", mockFolderPackager.LastFolderPacked);
            Assert.AreEqual(startTimestampExpected, mockFolderPackager.LastModifiedMinimumRequested);
            Assert.IsTrue(Util.ArraysEqual<byte>(folderDataExpected, mockDataConnection.ByteArraysSentQueue[0]));
        }

        [TestMethod]
        public void TestLastModifiedTimeMonitor()
        {
            VerifyMonitoring(new DateTime(0), MockFolderPackager.MockFolderData);
            VerifyMonitoring(new DateTime(2012, 10, 15), MockFolderPackager.MockFolderData);
        }

        private void VerifyPasswordResult(string correctPassword, string suppliedPassword, bool expectPasswordIsCorrect)
        {
            MockDataConnection mockDataConnection = new MockDataConnection();
            MockFolderPackager mockFolderPackager = new MockFolderPackager();
            MockClock serverClock = new MockClock(new DateTime(2007, 5, 25, 15, 27, 03));

            //
            // All that the server expects from the client is a read request byte and a minimum
            // modification timestamp.
            //

            mockDataConnection.BytesToReceiveQueue.Add(PeachFarmerProtocol.ReadRequest);
            mockDataConnection.DateTimesToReceiveQueue.Add(new DateTime(0));
            mockDataConnection.StringsToReceiveQueue.Add(suppliedPassword);

            FolderMonitor folderMonitor = new FolderMonitor(mockDataConnection, mockFolderPackager, serverClock, @"c:\fakepath\fakedir", correctPassword);
            folderMonitor.ProcessNextRequest();

            if (expectPasswordIsCorrect)
            {
                Assert.AreEqual(PeachFarmerProtocol.PasswordCorrect, mockDataConnection.BytesSentQueue[0]);
                Assert.IsTrue(mockDataConnection.ByteArraysSentQueue.Count > 0);
            }
            else
            {
                //
                // If password is incorrect, verify no data is sent to the client
                //

                Assert.AreEqual(PeachFarmerProtocol.PasswordIncorrect, mockDataConnection.BytesSentQueue[0]);
                Assert.AreEqual(0, mockDataConnection.ByteArraysSentQueue.Count);
            }
        }

        [TestMethod]
        public void TestPasswordValidation()
        {
            VerifyPasswordResult(CorrectPassword, CorrectPassword, true);
            VerifyPasswordResult(CorrectPassword, "", false);
            VerifyPasswordResult(CorrectPassword, "the_wr0ng_p@ssw0rd!", false);

            VerifyPasswordResult(null, "any_password", true);
            VerifyPasswordResult(null, null, true);
            VerifyPasswordResult(null, "", true);
        }
    }
}
