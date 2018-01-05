using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using Hypepool.Common.Stratum;
using Hypepool.Core.Utils.Buffers;
using Hypepool.Core.Utils.Time;
using Hypepool.Core.Utils.Unique;
using NetUV.Core.Handles;
using Serilog;

namespace Hypepool.Core.Stratum
{
    public class StratumServer : IStratumServer
    {
        public IReadOnlyDictionary<int, Tcp> Ports { get; }

        private IDictionary<int, Tcp> _ports { get; }

        private readonly ILogger _logger;

        public StratumServer()
        {
            _logger = Log.ForContext<StratumServer>().ForContext("Pool", "pool-name");

            _ports = new Dictionary<int, Tcp>();
            Ports = new ReadOnlyDictionary<int, Tcp>(_ports);
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
                _logger.Debug(
                    $"Accepting connection [{connectionId}] from {remoteEndPoint.Address}:{remoteEndPoint.Port}");

                connection.KeepAlive(true, 1); // allow keep-alive.

                var client = new StratumClient();

                client.Initialize(loop, connection, new StandardClock(), endpoint, connectionId,
                    data => OnReceive(client, data),
                    () => OnReceiveComplete(client),
                    ex => OnReceiveError(client, ex));

            }
            catch (Exception ex)
            {
                _logger.Error($"Client connection error: {ex.Message}");
            }
        }

        protected virtual void OnReceive(StratumClient client, PooledArraySegment<byte> data)
        {
        }

        protected virtual void OnReceiveError(StratumClient client, Exception ex)
        {
        }

        protected virtual void OnReceiveComplete(StratumClient client)
        {
        }
    }
}
