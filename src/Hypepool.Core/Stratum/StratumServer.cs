using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Hypepool.Common.Stratum;
using Hypepool.Core.JsonRpc;
using Hypepool.Core.Utils.Buffers;
using Hypepool.Core.Utils.Time;
using Hypepool.Core.Utils.Unique;
using NetUV.Core.Handles;
using NetUV.Core.Native;
using Serilog;

namespace Hypepool.Core.Stratum
{
    public class StratumServer : IStratumServer
    {
        public IReadOnlyDictionary<int, Tcp> Ports { get; }
        public IReadOnlyDictionary<string, IStratumClient> Clients { get; }

        private readonly IDictionary<int, Tcp> _ports;

        private readonly IDictionary<string, IStratumClient> _clients;

        private readonly ILogger _logger;

        public StratumServer()
        {
            _logger = Log.ForContext<StratumServer>().ForContext("Pool", "pool-name");

            _ports = new Dictionary<int, Tcp>();
            _clients = new Dictionary<string, IStratumClient>();

            Ports = new ReadOnlyDictionary<int, Tcp>(_ports);
            Clients = new ReadOnlyDictionary<string, IStratumClient>(_clients);
        }

        public void Initialize()
        {
            StartListeners();

            _logger.Verbose("Initialized stratum server");
        }

        private void StartListeners()
        {
            var thread = new Thread(_ => // every port gets serviced by a dedicated loop thread
                {
                    var loop = new Loop(); // libuv loop.

                    var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);

                    var listener = loop
                        .CreateTcp()
                        .NoDelay(true)
                        .SimultaneousAccepts(false)
                        .Listen(endpoint, (con, ex) =>
                        {
                            if (ex != null)
                                _logger.Error($"Connection error: {ex.Message}");
                            else
                                OnClientConnected(con, endpoint, loop);
                        });

                    // add to ports list.
                    lock (_ports)
                    {
                        _ports[endpoint.Port] = listener;
                    }

                    try
                    {
                        loop.RunDefault();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, Thread.CurrentThread.Name);
                    }
                })
                {Name = $"UVLoopThread"};

            thread.Start();
        }

        public void Stop()
        {
            lock (_ports)
            {
                var portValues = _ports.Values.ToArray();

                for (int i = 0; i < portValues.Length; i++)
                {
                    var listener = portValues[i];

                    listener.Shutdown((tcp, ex) =>
                    {
                        if (tcp?.IsValid == true)
                            tcp.Dispose();
                    });
                }
            }
        }

        private void OnClientConnected(Tcp connection, IPEndPoint endpoint, Loop loop)
        {
            try
            {
                var remoteEndPoint = connection.GetPeerEndPoint(); // get endpoint.
                var connectionId = CorrelationIdGenerator.GetNextId(); // get an unique id for connection.
                _logger.Debug($"Accepting connection [{connectionId}] from {remoteEndPoint.Address}:{remoteEndPoint.Port}");

                connection.KeepAlive(true, 1); // allow keep-alive.

                var client = new StratumClient();

                client.Initialize(loop, connection, new StandardClock(), endpoint, connectionId,
                    data => OnReceive(client, data),
                    () => OnReceiveComplete(client),
                    ex => OnReceiveError(client, ex));

                // register client
                lock (_clients)
                {
                    _clients[connectionId] = client;
                }

                OnConnect(client);
            }
            catch (Exception ex)
            {
                _logger.Error($"Client connection error: {ex.Message}");
            }
        }

        protected virtual void OnReceive(StratumClient client, PooledArraySegment<byte> data)
        {
            // get off of LibUV event-loop-thread immediately
            Task.Run(async () =>
            {
                using (data)
                {

                    JsonRpcRequest request = null;

                    // de-serialize
                    _logger.Verbose($"[{client.ConnectionId}] Received request data: {StratumConstants.Encoding.GetString(data.Array, 0, data.Size)}");
                    request = client.DeserializeRequest(data);

                    // dispatch
                    if (request != null)
                    {
                        _logger.Debug($"[{client.ConnectionId}] Dispatching request '{request.Method}' [{request.Id}]");
                        await OnRequestAsync(client, new Timestamped<JsonRpcRequest>(request, new StandardClock().Now));
                    }
                    else
                    {
                        _logger.Verbose($"[{client.ConnectionId}] Unable to deserialize request");
                    }                        
                }
            });
        }

        protected virtual void OnReceiveError(StratumClient client, Exception ex)
        {
            switch (ex)
            {
                case OperationException opEx:
                    // log everything but ECONNRESET which just indicates the client disconnecting
                    if (opEx.ErrorCode != ErrorCode.ECONNRESET)
                        _logger.Error($"[{client.ConnectionId}] Connection error state: {ex.Message}");
                    break;
                default:
                    _logger.Error($"[{client.ConnectionId}] Connection error state: {ex.Message}");
                    break;
            }

            DisconnectClient(client);
        }

        protected virtual void OnReceiveComplete(StratumClient client)
        {
            _logger.Debug($"[{client.ConnectionId}] Received EOF");
            DisconnectClient(client);
        }

        protected virtual void DisconnectClient(StratumClient client)
        {
            var subscriptionId = client.ConnectionId;

            client.Disconnect();

            if (!string.IsNullOrEmpty(subscriptionId))
            {
                // unregister client
                lock (_clients)
                {
                    _clients.Remove(subscriptionId);
                }
            }

            OnDisconnect(subscriptionId);
        }

        protected void ForEachClient(Action<IStratumClient> action)
        {
            IStratumClient[] clients;

            lock (_clients)
            {
                clients = _clients.Values.ToArray();
            }

            foreach (var client in clients)
            {
                try
                {
                    action(client);
                }

                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                }
            }
        }

        protected virtual void OnConnect(StratumClient client)
        {
        }

        protected virtual void OnDisconnect(string subscriptionId)
        {
        }

        protected Task OnRequestAsync(StratumClient client,
            Timestamped<JsonRpcRequest> request)
        {
            return null;
        }
    }
}
