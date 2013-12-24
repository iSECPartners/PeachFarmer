using RemoteHarvesterTest.MockObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerTest.MockObjects
{
    public class MockFile
    {
        public string Filename { get; private set; }

        public DateTime LastModifiedTime { get; set; }

        public MemoryStream DataStream { get; set; }

        public MockFile(string filename)
            :this(filename, DateTime.Now)
        {
            ;
        }

        public MockFile(string filename, DateTime lastModifiedTime)
        {
            Filename = filename;
            LastModifiedTime = lastModifiedTime;
            DataStream = new MockMemoryStream();
        }

        public override bool Equals(object obj)
        {
            return Equals((MockFile)obj);
        }

        public bool Equals(MockFile other)
        {
            if (string.CompareOrdinal(this.Filename, other.Filename) != 0)
            {
                return false;
            }

            if (!this.LastModifiedTime.Equals(other.LastModifiedTime))
            {
                return false;
            }

            byte[] dataBytes = this.DataStream.GetBuffer();
            byte[] otherDataBytes = other.DataStream.GetBuffer();

            return Util.ArraysEqual<byte>(dataBytes, otherDataBytes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
