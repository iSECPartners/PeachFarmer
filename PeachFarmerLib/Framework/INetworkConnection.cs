﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeachFarmerLib.Framework
{
    public interface INetworkConnection : IDisposable
    {
        Stream GetStream();

        void Close();
    }
}
