using System;
using System.Diagnostics.Contracts;
using System.Net;
using Hypepool.Common.JsonRpc;
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

        void Respond<T>(T payload, object id);


        void RespondError(StratumError code, string message, object id, object result = null, object data = null);


        void Respond<T>(JsonRpcResponse<T> response);


        void Notify<T>(string method, T payload);


        void Notify<T>(JsonRpcRequest<T> request);


        void RespondError(object id, int code, string message);


        void RespondUnsupportedMethod(object id);

        void RespondUnauthorized(object id);
    }
}
