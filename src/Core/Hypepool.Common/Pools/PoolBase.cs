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
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Shares;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Helpers.Time;
using Serilog;

namespace Hypepool.Common.Pools
{
    public abstract class PoolBase<TShare> : IPool where TShare : IShare
    {
        /// <summary>
        /// Initializes the pool.
        /// </summary>
        public abstract Task Initialize();

        /// <summary>
        /// Starts the pool.
        /// </summary>
        public abstract Task Start();

        /// <summary>
        /// Any checks before the initilization.
        /// </summary>
        /// <returns></returns>
        protected abstract Task RunPreInitChecksAsync();

        /// <summary>
        /// Any extra checks the blockchain may require after initilization.
        /// </summary>
        /// <returns></returns>
        protected abstract Task RunPostInitChecksAsync();

        /// <summary>
        /// Checks if we are connected to coin daemon.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> IsDaemonConnectionHealthyAsync();

        /// <summary>
        /// Checks if the coin daemon is connected to network.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> IsDaemonConnectedToNetworkAsync();

        /// <summary>
        /// Checks if coin is synched to network.
        /// </summary>
        /// <returns></returns>
        protected abstract Task EnsureDaemonSynchedAsync();

        /// <summary>
        /// Creates a context for client.
        /// </summary>
        /// <returns></returns>
        protected abstract WorkerContext CreateClientContext();

        /// <summary>
        /// Poll context.
        /// </summary>
        public IPoolContext PoolContext { get; protected set; }

        /// <summary>
        /// Server factory.
        /// </summary>
        protected readonly IServerFactory ServerFactory;

        protected ILogger _logger;

        protected PoolBase(IServerFactory serverFactory)
        {
            ServerFactory = serverFactory;
        }

        public void OnConnect(IStratumClient client)
        {
            var context = CreateClientContext(); // create worker context.
            context.Initialize(7500);
            client.SetContext(context);

            EnsureNoZombie(client); // make sure client communicates within a set period of time.
        }

        private void EnsureNoZombie(IStratumClient client)
        {
            Observable.Timer(MasterClock.Now.AddSeconds(10)) // wait 10 seconds.
                .Take(1)
                .Subscribe(_ =>
                {
                    if (client.LastReceive.HasValue) // if the worker has send some data,
                        return; // skip him.

                    _logger.Information($"[{client.ConnectionId}] Booting zombie-worker (post-connect silence)");
                    PoolContext.StratumServer.DisconnectClient(client); // else boot him from our server.
                });
        }

        public virtual void OnDisconnect(string subscriptionId)
        { }

        public abstract Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest);
    }
}
