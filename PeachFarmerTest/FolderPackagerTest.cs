using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeachFarmerLib.Framework;
using RemoteHarvester;
using PeachFarmerLib;
using PeachFarmerClient;
using PeachFarmerTest.MockObjects;

namespace RemoteHarvesterTest
{
    [TestClass]
    public class FolderPackagerTest
    {
        private static DateTime[] _checkpoints;

        public FolderPackagerTest()
        {
            _checkpoints = new DateTime[4];
            _checkpoints[0] = new DateTime(2012, 6, 12, 10, 09, 23);
            _checkpoints[1] = new DateTime(2012, 6, 12, 10, 12, 45);
            _checkpoints[2] = new DateTime(2012, 6, 12, 10, 15, 08);
            _checkpoints[3] = new DateTime(2012, 6, 12, 10, 15, 29);
        }

        private IFileSystem GenerateMockFileSystem()
        {
            MockFileSystem fileSystem = new MockFileSystem();

            fileSystem.AddMockFile(@"c:\logs\abc\status.txt", _checkpoints[1], "this is the oatmeal file!");
            fileSystem.AddMockFile(@"c:\logs\abc\Faults\1\oatmeal.txt", _checkpoints[0], "this is the oatmeal file!");
            fileSystem.AddMockFile(@"c:\logs\abc\Faults\2\potatoes.txt", _checkpoints[0], "here, have some potatoes");
            fileSystem.AddMockFile(@"c:\logs\abc\Faults\3\carrots.txt", _checkpoints[1], "carrots are orange");
            fileSystem.AddMockFile(@"c:\logs\abc\Reproducing\4\tomatoes.txt", _checkpoints[1], "tomatoes are nutritious");

            fileSystem.AddMockFile(@"c:\logs\def\status.txt", _checkpoints[3], "hamburgers without cheese are called hamburgers");
            fileSystem.AddMockFile(@"c:\logs\def\Faults\15\hamburgers.txt", _checkpoints[2], "hamburgers without cheese are called hamburgers");
            fileSystem.AddMockFile(@"c:\logs\def\Faults\18\lettuce.txt", _checkpoints[3], "let us eat lettuce");

            return fileSystem;
        }

        [TestMethod]
        public void TestSingleFolderNullModifiedDatePackUnpack()
        {
            IFileSystem mockFileSystem = GenerateMockFileSystem();

            PeachFolderPackager packager = new PeachFolderPackager(mockFileSystem);
            byte[] packedBytes = packager.PackFolder(@"c:\logs\abc", new DateTime(0));

            MockFileSystem destinationFileSystem = new MockFileSystem();
            FolderUnpacker unpackager = new FolderUnpacker(destinationFileSystem);
            unpackager.UnpackFolder(@"c:\collectedlogs", packedBytes);

            Assert.AreEqual(4, destinationFileSystem.GetTotalFileCount());
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\status.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\1\oatmeal.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\2\potatoes.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\3\carrots.txt"));
        }

        [TestMethod]
        public void TestMultiFolderNullModifiedDatePackUnpack()
        {
            IFileSystem mockFileSystem = GenerateMockFileSystem();

            PeachFolderPackager packager = new PeachFolderPackager(mockFileSystem);
            byte[] packedBytes = packager.PackFolder(@"c:\logs", new DateTime(0));

            MockFileSystem destinationFileSystem = new MockFileSystem();
            FolderUnpacker unpackager = new FolderUnpacker(destinationFileSystem);
            unpackager.UnpackFolder(@"c:\collectedlogs", packedBytes);

            Assert.AreEqual(6, destinationFileSystem.GetTotalFileCount());
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\status.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\1\oatmeal.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\2\potatoes.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\3\carrots.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\15\hamburgers.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\18\lettuce.txt"));
        }

        [TestMethod]
        public void TestDateSeparatedPackUnpack()
        {
            IFileSystem mockFileSystem = GenerateMockFileSystem();

            PeachFolderPackager packager = new PeachFolderPackager(mockFileSystem);
            byte[] packedBytes = packager.PackFolder(@"c:\logs\abc\", _checkpoints[0].ToUniversalTime());

            MockFileSystem destinationFileSystem = new MockFileSystem();
            FolderUnpacker unpackager = new FolderUnpacker(destinationFileSystem);
            unpackager.UnpackFolder(@"c:\collectedlogs\", packedBytes);

            //
            // Only 2 files should have been packed as they were the only ones modified after
            // checkpoint 0.
            //

            Assert.AreEqual(2, destinationFileSystem.GetTotalFileCount());
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\status.txt"));
            Assert.IsTrue(destinationFileSystem.FileExists(@"c:\collectedlogs\Faults\3\carrots.txt"));
        }

        [TestMethod]
        public void TestDateAllExcludedPackUnpack()
        {
            IFileSystem mockFileSystem = GenerateMockFileSystem();

            PeachFolderPackager packager = new PeachFolderPackager(mockFileSystem);
            byte[] packedBytes = packager.PackFolder(@"c:\logs\", _checkpoints[3].ToUniversalTime());

            MockFileSystem destinationFileSystem = new MockFileSystem();
            FolderUnpacker unpackager = new FolderUnpacker(destinationFileSystem);
            unpackager.UnpackFolder(@"c:\collectedlogs\", packedBytes);

            //
            // No files should have been packed as none were modified after checkpoint 3.
            //

            Assert.AreEqual(0, destinationFileSystem.GetTotalFileCount());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDirectoryTraversal()
        {
            //
            // This test should throw an exception because the package is attempting to cause FolderUnpacker to unpack outside of
            // the destination folder.
            //

            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.AddMockFile(@"c:\root\status.txt", new DateTime(2012, 5, 15, 2, 15, 08), "this is the status.");
            mockFileSystem.AddMockFile(@"c:\root\..\evil.dat", new DateTime(2012, 5, 15, 2, 15, 08), "this is an evil file that's attempting to do directory traversal.");

            PeachFolderPackager packager = new PeachFolderPackager(mockFileSystem);
            byte[] packedBytes = packager.PackFolder(@"c:\root", new DateTime(0));

            MockFileSystem destinationFileSystem = new MockFileSystem();
            FolderUnpacker unpackager = new FolderUnpacker(destinationFileSystem);
            unpackager.UnpackFolder(@"c:\collectedlogs", packedBytes);
        }
    }
}
