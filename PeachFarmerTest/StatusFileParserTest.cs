using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerClient;
using System.IO;
using System.Text;

namespace RemoteHarvesterTest
{
    [TestClass]
    public class StatusFileParserTest
    {

        private void VerifyParseEquals(string statusFileContents, PeachStatus statusExpected)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(statusFileContents)))
            {
                PeachStatus statusActual = new StatusFileParser().Parse(ms);
                Assert.AreEqual(statusExpected, statusActual);
            }
        }

        [TestMethod]
        public void IncompleteStatusTest()
        {
            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 11/19/2013 6:18:00 PM
Peach Version: 3.1.54.0
Seed: 0
Command line: test.xml Default 
Pit File: test.xml
. Test starting: Default

. Iteration 1 : 11/19/2013 6:18:10 PM
. Iteration 1 of 5000 : 11/19/2013 6:18:33 PM
! Fault detected at iteration 74 : 11/19/2013 6:27:22 PM
! Fault detected at iteration 74 : 11/19/2013 6:30:15 PM
. Iteration 100 of 5000 : 11/19/2013 6:33:15 PM
. Iteration 200 of 5000 : 11/19/2013 6:45:26 PM
. Iteration 300 of 5000 : 11/19/2013 6:57:22 PM";

            PeachStatus statusExpected = new PeachStatus(299, 5000, false, new DateTime(2013, 11, 19, 18, 57, 22));
            VerifyParseEquals(fileContents, statusExpected);
        }

        [TestMethod]
        public void NoActivityCompleteTest()
        { 

            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 11/23/2013 11:50:34 AM
Peach Version: 3.1.54.0
Seed: 0
Command line: --definedvalues=test.conf.xml --seed=0 test.xml --range=19801,19805 Default 
Pit File: test.xml
. Test starting: Default

. Test finished: Default";

            PeachStatus statusExpected = new PeachStatus(null, null, true, null);
            VerifyParseEquals(fileContents, statusExpected);
        }

        [TestMethod]
        public void SomeActivityCompleteTest()
        {

            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 11/23/2013 11:54:05 AM
Peach Version: 3.1.54.0
Seed: 0
Command line: --definedvalues=test.conf.xml --seed=0 test.xml --range=0,5 Default 
Pit File: test.xml
. Test starting: Default

. Iteration 1 : 11/23/2013 11:54:06 AM
. Iteration 1 of 5 : 11/23/2013 11:54:22 AM
. Test finished: Default
";

            PeachStatus statusExpected = new PeachStatus(0, 5, true, new DateTime(2013, 11, 23, 11, 54, 22));
            VerifyParseEquals(fileContents, statusExpected);
        }

        [TestMethod]
        public void LargeLastIterationTest()
        {

            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 11/23/2013 11:54:05 AM
Peach Version: 3.1.54.0
Seed: 0
Command line: --definedvalues=test.conf.xml --seed=0 test.xml --skipto=4294967296 Default 
Pit File: test.xml
. Test starting: Default

. Iteration 4294967296 : 11/23/2013 11:54:06 AM
. Test finished: Default
";

            PeachStatus statusExpected = new PeachStatus(4294967295, null, true, new DateTime(2013, 11, 23, 11, 54, 06));
            VerifyParseEquals(fileContents, statusExpected);
        }

        [TestMethod]
        public void LargeFinalIterationTest()
        {

            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 11/23/2013 11:54:05 AM
Peach Version: 3.1.54.0
Seed: 0
Command line: --definedvalues=test.conf.xml --seed=0 test.xml --skipto=4294967296 Default 
Pit File: test.xml
. Test starting: Default

. Iteration 1 : 11/23/2013 11:54:06 AM
. Iteration 1 of 4294967296 : 11/23/2013 11:54:22 AM
. Test finished: Default
";

            PeachStatus statusExpected = new PeachStatus(0, 4294967296, true, new DateTime(2013, 11, 23, 11, 54, 22));
            VerifyParseEquals(fileContents, statusExpected);
        }

        [TestMethod]
        public void IncompleteFoundFaultsTest()
        {

            const string fileContents = @"Peach Fuzzing Run
=================

Date of run: 10/11/2013 10:49:34 AM
Peach Version: 3.0.5007.24385
Seed: 0
Command line: --definedvalues=rtsp.conf.xml --seed=0 --range=0,50000 rtsp.xml Vlc 
Pit File: rtsp.xml
. Test starting: Vlc

. Iteration 1 : 10/11/2013 10:49:41 AM
. Iteration 1 of 50000 : 10/11/2013 10:50:09 AM
! Fault detected at iteration 5 : 10/11/2013 10:50:25 AM
! Fault detected at iteration 5 : 10/11/2013 10:50:33 AM
! Fault detected at iteration 8 : 10/11/2013 10:50:41 AM
! Fault detected at iteration 8 : 10/11/2013 10:50:51 AM";

            PeachStatus statusExpected = new PeachStatus(8, 50000, false, new DateTime(2013, 10, 11, 10, 50, 51));
            VerifyParseEquals(fileContents, statusExpected);
        }
    }
}
