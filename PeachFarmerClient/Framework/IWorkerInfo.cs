using System;

namespace PeachFarmerClient.Framework
{
    public interface IWorkerInfo
    {
        string Id { get; }

        string RemoteAddress { get; }

        DateTime LastPullTime { get; }
    }
}
