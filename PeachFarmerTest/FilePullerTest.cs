﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerClient;
using PeachFarmerTest;
using PeachFarmerTest.MockObjects;
using PeachFarmerLib;
using System.IO;
using PeachFarmerLib.Messages;
using RemoteHarvesterTest.TestStreams;

namespace PeachFarmerClientTest
{
    [TestClass]
    public class FilePullerTest
    {
        private Stream ServerResponseToStream(ReadResponseMessage responseMessage)
        {
            ReadingStream responseStream = new ReadingStream();
            FarmerMessageSerializer serializer = new FarmerMessageSerializer();
            serializer.Serialize(responseStream, responseMessage);

            responseStream.Seek(0, SeekOrigin.Begin);
            responseStream.StopProcessingWrites();

            return responseStream;
        }

        [TestMethod]
        public void TestPull()
        {
            ReadResponseMessage readResponse = new ReadResponseMessage();
            readResponse.IsPasswordCorrect = true;
            readResponse.CurrentServerTimeUtc = new DateTime(0);
            readResponse.Data = new byte[] { 0xaa, 0xbb };

            MockFolderUnpacker folderUnpacker = new MockFolderUnpacker();
            PullHistory pullHistory = new PullHistory(new MockFileSystem(), "c:\\pullhistory.dat");

            using (Stream responseStream = ServerResponseToStream(readResponse))
            {
                FilePuller filePuller = new FilePuller(responseStream, folderUnpacker, pullHistory, "the_r1ght_passw0rd");

                const string destinationFolderExpected = @"c:\pulledfiles\";

                filePuller.Pull("dummyhost", destinationFolderExpected);

                Assert.AreEqual(destinationFolderExpected, folderUnpacker.LastDestinationFolder);

                //
                // The file puller should treat the first four bytes as the data length (2) and last two bytes as the data
                //

                Assert.IsTrue(Util.ArraysEqual<byte>(new byte[] { 0xaa, 0xbb }, folderUnpacker.LastPackedData));
            }
        }

        [TestMethod]
        public void TestInvalidPasswordPull()
        {
            ReadResponseMessage serverResponseMessage = new ReadResponseMessage();
            serverResponseMessage.IsPasswordCorrect = false;

            using (Stream responseStream = ServerResponseToStream(serverResponseMessage))
            {
                MockFolderUnpacker folderUnpacker = new MockFolderUnpacker();
                PullHistory pullHistory = new PullHistory(new MockFileSystem(), "c:\\pullhistory.dat");

                FilePuller filePuller = new FilePuller(responseStream, folderUnpacker, pullHistory, "the_wr0ng_passw0rd");
                filePuller.Pull("dummyhost", @"c:\fakedestination");

                //
                // Verify that client gracefully handles rejected passwords and does not try to download more data
                //

                Assert.IsNull(folderUnpacker.LastDestinationFolder);
                Assert.IsNull(folderUnpacker.LastPackedData);
            }
        }
    }
}
