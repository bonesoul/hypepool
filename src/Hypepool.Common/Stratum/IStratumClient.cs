using System;
using System.Net;

namespace Hypepool.Common.Stratum
{
    public interface IStratumClient
    {
        IPEndPoint LocalEndpoint { get; }

        IPEndPoint RemoteEndpoint { get; }

        string ConnectionId { get; }

        bool IsAlive { get; }

        DateTime? LastReceive { get; }

        void Disconnect();
    }
}
