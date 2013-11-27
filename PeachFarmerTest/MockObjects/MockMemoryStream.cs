using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvesterTest.MockObjects
{
    public class MockMemoryStream : MemoryStream
    {
        public override void Close()
        {
            Seek(0, SeekOrigin.Begin);
        }
    }
}
