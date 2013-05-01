using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerClient;
using PeachFarmerTest;
using PeachFarmerTest.MockObjects;
using PeachFarmerLib;

namespace PeachFarmerClientTest
{
    [TestClass]
    public class FilePullerTest
    {
        [TestMethod]
        public void TestPull()
        {
            MockDataConnection dataConnection = new MockDataConnection();

            //
            // Protocol is:
            // C -> S: Read request byte
            // C -> S: Password length + password
            // S -> C: Password check result
            // C -> S: Min modify time
            // S -> C: Current server time
            // S -> C: Data length
            // S -> C: Data
            //
            // so the mock data connection has the server side messages queued up so they
            // will be fed to the client when the client accesses those APIs.

            dataConnection.BytesToReceiveQueue.Add(PeachFarmerProtocol.PasswordCorrect);
            dataConnection.DateTimesToReceiveQueue.Add(new DateTime(0));
            dataConnection.IntsToReceiveQueue.Add(2);
            dataConnection.ByteArraysToReceiveQueue.Add(new byte[] { 0xaa, 0xbb });

            MockFolderUnpacker folderUnpacker = new MockFolderUnpacker();
            PullHistory pullHistory = new PullHistory();

            FilePuller filePuller = new FilePuller(dataConnection, folderUnpacker, pullHistory, "the_r1ght_passw0rd");

            const string destinationFolderExpected = @"c:\pulledfiles\";

            filePuller.Pull("dummyhost", destinationFolderExpected);

            Assert.AreEqual(destinationFolderExpected, folderUnpacker.LastDestinationFolder);

            //
            // The file puller should treat the first four bytes as the data length (2) and last two bytes as the data
            //

            Assert.IsTrue(Util.ArraysEqual<byte>(new byte[] { 0xaa, 0xbb }, folderUnpacker.LastPackedData));
        }

        [TestMethod]
        public void TestInvalidPasswordPull()
        {
            MockDataConnection dataConnection = new MockDataConnection();

            //
            // Server only responds to reject the password.
            //

            dataConnection.BytesToReceiveQueue.Add(PeachFarmerProtocol.PasswordIncorrect);

            MockFolderUnpacker folderUnpacker = new MockFolderUnpacker();
            PullHistory pullHistory = new PullHistory();

            FilePuller filePuller = new FilePuller(dataConnection, folderUnpacker, pullHistory, "the_wr0ng_passw0rd");
            filePuller.Pull("dummyhost", @"c:\fakedestination");

            //
            // Verify that client gracefully handles rejected passwords and does not try to download more data
            //

            Assert.IsNull(folderUnpacker.LastPackedData);
        }
    }
}
