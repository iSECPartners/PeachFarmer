using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerClient;
using PeachFarmerTest;
using PeachFarmerTest.MockObjects;
using PeachFarmerLib;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeachFarmerLib.Framework;
using PeachFarmerLib.Messages;

namespace PeachFarmerClientTest
{
    [TestClass]
    public class FarmerMessageSerializerTest
    {

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            ReadRequestMessage messageOriginal = new ReadRequestMessage();
            messageOriginal.LastCheckTimeUtc = new DateTime(2012, 5, 27);
            messageOriginal.ServerPassword = "";

            FarmerMessageSerializer serializer = new FarmerMessageSerializer();
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, messageOriginal);

            stream.Seek(0, SeekOrigin.Begin);
            ReadRequestMessage messageDeserialized = (ReadRequestMessage)serializer.Deserialize(stream);

            Assert.AreEqual(messageOriginal, messageDeserialized);
        }
    }
}
