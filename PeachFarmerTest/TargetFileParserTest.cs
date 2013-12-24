using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerClient;
using PeachFarmerTest;
using PeachFarmerTest.MockObjects;
using PeachFarmerLib;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeachFarmerClientTest
{
    [TestClass]
    public class TargetFileParserTest
    {
        private void VerifyParseEquals(string fileContents, string[] resultsExpected)
        {
            List<string> resultsExpectedList = new List<string>(resultsExpected);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
            {
                List<string> resultsActual = TargetFileParser.ParseTargets(ms);
                Assert.IsTrue(Util.ListsEqual(resultsExpectedList, resultsActual));
            }
        }

        [TestMethod]
        public void TestTargetFileParse()
        {
            VerifyParseEquals("", new string[] { });
            VerifyParseEquals("domain1.com", new string[] { "domain1.com" });
            VerifyParseEquals("domain1.com\r\ndomain2.com", new string[] {"domain1.com", "domain2.com"});
            VerifyParseEquals("domain1.com\ndomain2.com", new string[] { "domain1.com", "domain2.com" }); // handle Unix-style newlines
            VerifyParseEquals("domain1.com\r\n\r\ndomain2.com", new string[] { "domain1.com", "domain2.com" }); // ignore empty lines
            VerifyParseEquals("\r\ndomain1.com\r\ndomain2.com", new string[] { "domain1.com", "domain2.com" }); // ignore empty lines

            // ignore comments after the hostname
            VerifyParseEquals("domain1.com # this is the best domain\r\ndomain2.com # this domain isn't so good", new string[] { "domain1.com", "domain2.com" });
        }
    }
}
