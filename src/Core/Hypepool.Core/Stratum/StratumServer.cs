﻿#region license
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Pools;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Buffers;
using Hypepool.Common.Utils.Extensions;
using Hypepool.Common.Utils.Helpers.Time;
using Hypepool.Core.Utils.Unique;
using NetUV.Core.Handles;
using NetUV.Core.Native;
using Newtonsoft.Json;
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

        private IPool _pool;

        public StratumServer()
        {
            _logger = Log.ForContext<StratumServer>().ForContext("Pool", "XMR");

            _ports = new Dictionary<int, Tcp>();
            _clients = new Dictionary<string, IStratumClient>();

            Ports = new ReadOnlyDictionary<int, Tcp>(_ports);
            Clients = new ReadOnlyDictionary<string, IStratumClient>(_clients);
        }

        public void Start(IPool pool)
        {
            _pool = pool;

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

                    _logger.Information($"stratum server [{endpoint.ToString()}] started..");

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

                client.Initialize(loop, connection, endpoint, connectionId,
                    data => OnReceive(client, data),
                    () => OnReceiveComplete(client),
                    ex => OnReceiveError(client, ex));

                // register client
                lock (_clients)
                {
                    _clients[connectionId] = client;
                }

                _pool.OnConnect(client);
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

                    try
                    {
                        // de-serialize
                        _logger.Verbose($"[{client.ConnectionId}] Received request data:\n {StratumConstants.Encoding.GetString(data.Array, 0, data.Size).FormatJson()}");
                        request = client.DeserializeRequest(data);

                        // dispatch
                        if (request != null)
                        {
                            _logger.Debug($"[{client.ConnectionId}] Dispatching request '{request.Method}' [{request.Id}]");
                            await _pool.OnRequestAsync(client, new Timestamped<JsonRpcRequest>(request, MasterClock.Now));
                        }
                        else
                        {
                            _logger.Verbose($"[{client.ConnectionId}] Unable to deserialize request");
                        }
                    }
                    catch (JsonReaderException jsonEx)
                    {
                        // junk received (no valid json)
                        _logger.Error($"[{client.ConnectionId}] Connection json error state: {jsonEx.Message}");
                    }

                    catch (Exception ex)
                    {
                        if (request != null)
                            _logger.Error(ex, $"[{client.ConnectionId}] Error processing request {request.Method} [{request.Id}]");
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

        public void DisconnectClient(IStratumClient client)
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

            _pool.OnDisconnect(subscriptionId);
        }

        public void ForEachClient(Action<IStratumClient> action)
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
    }
}
