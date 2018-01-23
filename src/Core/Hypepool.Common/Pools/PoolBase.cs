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
        protected readonly IPoolContext PoolContext;
        protected readonly IServerFactory ServerFactory;
        protected ILogger _logger;

        public abstract void Initialize();

        protected abstract Task<bool> IsDaemonConnectionHealthy();

        protected abstract Task<bool> IsDaemonConnectedToNetwork();

        protected abstract Task EnsureDaemonSynchedAsync();

        /// <summary>
        /// Creates a context for client.
        /// </summary>
        /// <returns></returns>
        protected abstract WorkerContext CreateClientContext();

        protected PoolBase(IPoolContext poolContext, IServerFactory serverFactory)
        {
            PoolContext = poolContext;
            ServerFactory = serverFactory;
        }

        public virtual void Start()
        {
            _logger.Information($"Loading pool..");

            try
            {
                PoolContext.DaemonClient.Initialize("127.0.0.1", 28081, "user", "pass", "json_rpc");
                WaitDaemon();

                PoolContext.JobManager.Initialize(PoolContext);
                PoolContext.JobManager.Start();
                PoolContext.StratumServer.Start(this);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
        }

        public async void WaitDaemon()
        {
            while (!await IsDaemonConnectionHealthy())
            {
                _logger.Information($"Waiting for daemons to come online ...");
            }
        }

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
                    PoolContext.StratumServer.DisconnectClient(client);
                });
        }

        public virtual void OnDisconnect(string subscriptionId)
        { }

        public abstract Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest);
    }
}
