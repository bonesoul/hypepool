#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion
using System;
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
