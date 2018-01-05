using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
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

        private ConcurrentQueue<PooledArraySegment<byte>> _sendQueue;
        private Async _sendQueueDrainer;
        private readonly ILogger _logger;

        public StratumClient()
        {
            _logger = Log.ForContext<StratumClient>().ForContext("Pool", "pool-name");
        }

        public void Initialize(Loop loop, Tcp tcp, IMasterClock clock, IPEndPoint endpointConfig, string connectionId,
            Action<PooledArraySegment<byte>> onNext, Action onCompleted, Action<Exception> onError)
        {
            LocalEndpoint = endpointConfig;            
            RemoteEndpoint = tcp.GetPeerEndPoint();
            ConnectionId = connectionId;

            // initialize send queue
            _sendQueue = new ConcurrentQueue<PooledArraySegment<byte>>();
            _sendQueueDrainer = loop.CreateAsync(DrainSendQueue);
            _sendQueueDrainer.UserToken = tcp;
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
