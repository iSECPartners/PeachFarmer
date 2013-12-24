using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerClient.Framework
{
    public interface IFilePuller
    {
        void Pull(Stream serverStream, string destinationFolder);

        DateTime LastPullTime { get; }

        Stream GetStatusFileStream();
    }
}
