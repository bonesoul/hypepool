using System;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.Shares;
using Hypepool.Common.Stratum;

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
            throw new NotImplementedException();
        }
    }
}
