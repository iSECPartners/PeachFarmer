using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteHarvester;
using System.Collections;
using PeachFarmerLib;
using System.IO;
using System.Collections.Generic;
using PeachFarmerTest.MockObjects;
using PeachFarmerTest;
using PeachFarmerLib.Messages;
using RemoteHarvesterTest.TestStreams;

namespace RemoteHarvesterTest
{
    [TestClass]
    public class FolderMonitorTest
    {
        const string CorrectPassword = "the_r1ght_passw0rd";

        private BookmarkableStream ClientRequestToStream(ReadRequestMessage requestMessage)
        {
            BookmarkableStream requestStream = new BookmarkableStream();
            FarmerMessageSerializer serializer = new FarmerMessageSerializer();
            serializer.Serialize(requestStream, requestMessage);
            
            requestStream.SetBookmark();
            requestStream.Seek(0, SeekOrigin.Begin);

            return requestStream;
        }

        private void VerifyMonitoring(DateTime startTimestampExpected, byte[] folderDataExpected)
        {
            MockFolderPackager mockFolderPackager = new MockFolderPackager();
            MockClock serverClock = new MockClock(new DateTime(2007, 5, 25, 15, 27, 03));

            ReadRequestMessage readRequest = new ReadRequestMessage(startTimestampExpected, CorrectPassword);

            using (BookmarkableStream clientStream = ClientRequestToStream(readRequest))
            {
                MockNetworkConnection networkConnection = new MockNetworkConnection(clientStream);
                FolderMonitor folderMonitor = new FolderMonitor(networkConnection, mockFolderPackager, serverClock, @"c:\fakepath\fakedir", CorrectPassword);
                folderMonitor.ProcessNextRequest();
                Assert.AreEqual(@"c:\fakepath\fakedir", mockFolderPackager.LastFolderPacked);
                Assert.AreEqual(startTimestampExpected, mockFolderPackager.LastModifiedMinimumRequested);

                clientStream.ResetToBookmark();
                FarmerMessageSerializer deserializer = new FarmerMessageSerializer();
                ReadResponseMessage response = (ReadResponseMessage)deserializer.Deserialize(clientStream);

                Assert.AreEqual(serverClock.GetCurrentTimeUtc(), response.CurrentServerTimeUtc);
                Assert.IsTrue(Util.ArraysEqual<byte>(folderDataExpected, response.Data));
            }
        }

        [TestMethod]
        public void TestLastModifiedTimeMonitor()
        {
            VerifyMonitoring(new DateTime(0), MockFolderPackager.MockFolderData);
            VerifyMonitoring(new DateTime(2012, 10, 15), MockFolderPackager.MockFolderData);
        }

        private void VerifyPasswordResult(string correctPassword, string suppliedPassword, bool expectPasswordIsCorrect)
        {
            ReadRequestMessage readRequest = new ReadRequestMessage(new DateTime(0), suppliedPassword);

            using (BookmarkableStream clientStream = ClientRequestToStream(readRequest))
            {
                MockNetworkConnection networkConnection = new MockNetworkConnection(clientStream);
                MockFolderPackager mockFolderPackager = new MockFolderPackager();
                MockClock serverClock = new MockClock(new DateTime(2007, 5, 25, 15, 27, 03));

                FolderMonitor folderMonitor = new FolderMonitor(networkConnection, mockFolderPackager, serverClock, @"c:\fakepath\fakedir", correctPassword);
                folderMonitor.ProcessNextRequest();

                clientStream.ResetToBookmark();
                FarmerMessageSerializer deserializer = new FarmerMessageSerializer();
                ReadResponseMessage readResponse = (ReadResponseMessage)deserializer.Deserialize(clientStream);

                Assert.AreEqual(expectPasswordIsCorrect, readResponse.IsPasswordCorrect);

                if (expectPasswordIsCorrect)
                {
                    Assert.AreEqual(serverClock.GetCurrentTimeUtc(), readResponse.CurrentServerTimeUtc);
                    Assert.IsNotNull(readResponse.Data);
                }
                else
                {
                    Assert.IsNull(readResponse.Data);
                }
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
