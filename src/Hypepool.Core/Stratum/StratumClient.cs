using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Disposables;
using System.Text;
using Hypepool.Common.Stratum;
using Hypepool.Core.Utils.Buffers;
using Hypepool.Core.Utils.Time;
using NetUV.Core.Handles;
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

        private const int MaxInboundRequestLength = 8192;
        private const int MaxOutboundRequestLength = 0x4000;

        public StratumClient()
        {
            _logger = Log.ForContext<StratumClient>().ForContext("Pool", "pool-name");
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

        private void DrainSendQueue(Async handle)
        {
            try
            {
                var tcp = (Tcp) handle.UserToken;

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
    }
}
