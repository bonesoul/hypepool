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
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Reactive.Disposables;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Buffers;
using Hypepool.Common.Utils.Helpers.Time;
using Hypepool.Core.Utils.Buffers;
using Hypepool.Core.Utils.Extensions;
using NetUV.Core.Handles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Hypepool.Core.Stratum
{
    public class StratumClient : IStratumClient
    {
        public IPEndPoint LocalEndpoint { get; private set; }

        public IPEndPoint RemoteEndpoint { get; private set; }

        public string ConnectionId { get; private set; }

        public bool IsAlive { get; private set; }

        public DateTime? LastReceive { get; private set; }

        private ConcurrentQueue<PooledArraySegment<byte>> _sendQueue;
        private Async _sendQueueDrainer;
        private IDisposable _subscription;
        private readonly ILogger _logger;
        private readonly PooledLineBuffer _pooledLineBuffer;
        private WorkerContext _workerContext;

        private const int MaxInboundRequestLength = 8192;
        private const int MaxOutboundRequestLength = 0x4000;

        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public StratumClient()
        {
            _logger = Log.ForContext<StratumClient>().ForContext("Pool", "XMR");            
            _pooledLineBuffer = new PooledLineBuffer(MaxInboundRequestLength);
        }

        public void Initialize(Loop loop, Tcp tcp, IMasterClock clock, IPEndPoint endpointConfig, string connectionId,
            Action<PooledArraySegment<byte>> onNext, Action onCompleted, Action<Exception> onError)
        {
            IsAlive = true;

            LocalEndpoint = endpointConfig;
            RemoteEndpoint = tcp.GetPeerEndPoint();
            ConnectionId = connectionId;

            // initialize send queue
            _sendQueue = new ConcurrentQueue<PooledArraySegment<byte>>();
            _sendQueueDrainer = loop.CreateAsync(DrainSendQueue);
            _sendQueueDrainer.UserToken = tcp;

            // cleanup preparation
            var sub = Disposable.Create(() =>
            {
                if (tcp.IsValid)
                {
                    _logger.Debug($"[{ConnectionId}] Last subscriber disconnected from receiver stream");

                    IsAlive = false;
                    tcp.Shutdown();
                }
            });

            // ensure subscription is disposed on loop thread
            var disposer = loop.CreateAsync((handle) =>
            {
                sub.Dispose();
                handle.Dispose();
            });

            _subscription = Disposable.Create(() => { disposer.Send(); });

            Receive(tcp, clock, onNext, onCompleted, onError); // start recieving.
        }

        public void SetContext<T>(T value) where T : WorkerContext
        {
            _workerContext = value;
        }

        public T GetContextAs<T>() where T : WorkerContext
        {
            return (T)_workerContext;
        }

        private void Receive(Tcp tcp, IMasterClock clock,
            Action<PooledArraySegment<byte>> onNext, Action onCompleted, Action<Exception> onError)
        {
            tcp.OnRead(
                (handle, buffer) => // onAccept
                {
                    using (buffer)
                    {
                        if (buffer.Count == 0 || !IsAlive)
                            return;

                        LastReceive = clock.Now;

                        _pooledLineBuffer.Receive(buffer, buffer.Count,
                            (src, dst, count) => src.ReadBytes(dst, count),
                            onNext, onError);
                    }
                },
                (handle, ex) => // onError
                {                   
                    onError(ex);
                },
                handle => // onCompleted
                {
                    IsAlive = false;
                    onCompleted();

                    // release handles
                    _sendQueueDrainer.UserToken = null;
                    _sendQueueDrainer.Dispose();

                    // empty queues
                    while (_sendQueue.TryDequeue(out var fragment))
                        fragment.Dispose();

                    _pooledLineBuffer.Dispose();

                    handle.CloseHandle();
                });
        }

        public void Send<T>(T payload)
        {
            if (IsAlive)
            {
                var buf = ArrayPool<byte>.Shared.Rent(MaxOutboundRequestLength);

                try
                {
                    using (var stream = new MemoryStream(buf, true))
                    {
                        stream.SetLength(0);
                        int size;

                        using (var writer = new StreamWriter(stream, StratumConstants.Encoding))
                        {
                            Serializer.Serialize(writer, payload);
                            writer.Flush();

                            // append newline
                            stream.WriteByte(0xa);
                            size = (int)stream.Position;
                        }

                        _logger.Verbose($"[{ConnectionId}] Sending:\n {StratumConstants.Encoding.GetString(buf, 0, size).FormatJson()}");

                        SendInternal(new PooledArraySegment<byte>(buf, 0, size));
                    }
                }

                catch (Exception)
                {
                    ArrayPool<byte>.Shared.Return(buf);
                    throw;
                }
            }
        }

        public void Respond<T>(T payload, object id)
        {
            Respond(new JsonRpcResponse<T>(payload, id));
        }

        public void RespondError(StratumError code, string message, object id, object result = null, object data = null)
        {
            Respond(new JsonRpcResponse(new JsonRpcException((int)code, message, null), id, result));
        }

        public void Respond<T>(JsonRpcResponse<T> response)
        {

            Send(response);
        }

        public void Notify<T>(string method, T payload)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(method), $"{nameof(method)} must not be empty");

            Notify(new JsonRpcRequest<T>(method, payload, null));
        }

        public void Notify<T>(JsonRpcRequest<T> request)
        {
            Send(request);
        }

        public void RespondError(object id, int code, string message)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(message), $"{nameof(message)} must not be empty");

            Respond(new JsonRpcResponse(new JsonRpcException(code, message, null), id));
        }

        public void RespondUnsupportedMethod(object id)
        {
            RespondError(id, (int)StratumError.Other, "Unsupported method");
        }

        public void RespondUnauthorized(object id)
        {
            RespondError(id, (int)StratumError.UnauthorizedWorker, "Unauthorized worker");
        }

        private void SendInternal(PooledArraySegment<byte> buffer)
        {
            try
            {
                _sendQueue.Enqueue(buffer);
                _sendQueueDrainer.Send();
            }

            catch (ObjectDisposedException)
            {
                buffer.Dispose();
            }
        }

        public void Disconnect()
        {
            _subscription?.Dispose();
            _subscription = null;

            IsAlive = false;
        }

        public JsonRpcRequest DeserializeRequest(PooledArraySegment<byte> data)
        {
            using (var stream = new MemoryStream(data.Array, data.Offset, data.Size))
            {
                using (var reader = new StreamReader(stream, StratumConstants.Encoding))
                {
                    using (var jreader = new JsonTextReader(reader))
                    {
                        return Serializer.Deserialize<JsonRpcRequest>(jreader);
                    }
                }
            }
        }

        private void DrainSendQueue(Async handle)
        {
            try
            {
                var tcp = (Tcp)handle.UserToken;

                if (tcp?.IsValid == true && !tcp.IsClosing && tcp.IsWritable && _sendQueue != null)
                {
                    var queueSize = _sendQueue.Count;
                    if (queueSize >= 256)
                        _logger.Warning($"[{ConnectionId}] Send queue backlog now at {queueSize}");

                    while (_sendQueue.TryDequeue(out var segment))
                    {
                        using (segment)
                        {
                            tcp.QueueWrite(segment.Array, 0, segment.Size);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
    }
}
