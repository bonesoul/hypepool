using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Shares;
using Hypepool.Common.Stratum;
using Hypepool.Common.Utils.Time;
using Serilog;

namespace Hypepool.Common.Pools
{
    public abstract class PoolBase<TShare> : IPool where TShare : IShare
    {
        public IStratumServer StratumServer { get; }

        private readonly IServerFactory _serverFactory;
        protected ILogger _logger;

        protected PoolBase(IServerFactory serverFactory)
        {
            _serverFactory = serverFactory;

            StratumServer = serverFactory.GetStratumServer();
        }

        public virtual async Task StartAsync()
        {
            _logger.Information($"Loading pool..");

            try
            {
                StratumServer.Start(this);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Creates a context for client.
        /// </summary>
        /// <returns></returns>
        protected abstract WorkerContext CreateClientContext();

        /// <summary>
        /// Setups job manager.
        /// </summary>
        /// <returns></returns>
        protected abstract Task SetupJobManager();

        public void OnConnect(IStratumClient client)
        {
            var context = CreateClientContext(); // create worker context.
            context.Initialize(7500, new StandardClock());
            client.SetContext(context);

            EnsureNoZombie(client); // make sure client communicates within a set period of time.
        }

        private void EnsureNoZombie(IStratumClient client)
        {
            Observable.Timer(new StandardClock().Now.AddSeconds(10))
                .Take(1)
                .Subscribe(_ =>
                {
                    if (client.LastReceive.HasValue)
                        return;

                    _logger.Information($"[{client.ConnectionId}] Booting zombie-worker (post-connect silence)");
                    StratumServer.DisconnectClient(client);
                });
        }

        public virtual void OnDisconnect(string subscriptionId)
        { }

        public abstract Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest);
    }
}
