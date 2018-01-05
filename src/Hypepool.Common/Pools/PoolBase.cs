using System;
using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Shares;
using Hypepool.Common.Stratum;
using Serilog;

namespace Hypepool.Common.Pools
{
    public abstract class PoolBase<TShare> : IPool where TShare : IShare
    {
        public IStratumServer StratumServer { get; }

        private readonly IServerFactory _serverFactory;

        protected PoolBase(IServerFactory serverFactory)
        {
            _serverFactory = serverFactory;

            StratumServer = serverFactory.GetStratumServer();
        }

        public virtual void Initialize()
        {
            StratumServer.Initialize(this);
        }

        public abstract void OnConnect(IStratumClient client);

        public virtual void OnDisconnect(string subscriptionId)
        { }

        public abstract Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> request);
    }
}
