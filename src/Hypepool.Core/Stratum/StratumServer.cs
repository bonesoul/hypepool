﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;
using Hypepool.Common.Stratum;
using NetUV.Core.Handles;

namespace Hypepool.Core.Stratum
{
    public class StratumServer : IStratumServer
    {
        public IReadOnlyDictionary<int, Tcp> Ports { get; }

        private IDictionary<int, Tcp> _ports { get; }

        public StratumServer()
        {
            _ports = new Dictionary<int, Tcp>();
            Ports = new ReadOnlyDictionary<int, Tcp>(_ports);
        }

        public void Initialize()
        {
            StartListeners();
        }

        private void StartListeners()
        {
            var thread = new Thread(_ => // every port gets serviced by a dedicated loop thread
                {
                    var loop = new Loop(); // libuv loop.

                    var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6667);

                    var listener = loop
                        .CreateTcp()
                        .NoDelay(true)
                        .SimultaneousAccepts(false)
                        .Listen(endpoint, (con, ex) =>
                        {
                            if (ex == null)
                                OnClientConnected(con, endpoint, loop);
                            //else
                                //logger.Error(() => $"[{LogCat}] Connection error state: {ex.Message}");
                        });

                    // add to ports list.
                    lock (_ports) 
                    {
                        _ports[endpoint.Port] = listener;
                    }

                })
                {Name = $"UVLoopThread"};
        }

        private void OnClientConnected(Tcp connection, IPEndPoint endpoint, Loop loop)
        {
        }
    }
}
