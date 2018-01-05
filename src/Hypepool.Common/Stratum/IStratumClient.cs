using System;
using System.Net;
using Hypepool.Common.Mining.Context;

namespace Hypepool.Common.Stratum
{
    public interface IStratumClient
    {
        IPEndPoint LocalEndpoint { get; }

        IPEndPoint RemoteEndpoint { get; }

        void SetContext<T>(T value) where T : WorkerContext;

        T GetContextAs<T>() where T : WorkerContext;

        string ConnectionId { get; }

        bool IsAlive { get; }

        DateTime? LastReceive { get; }

        void Disconnect();
    }
}
