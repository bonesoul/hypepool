using System;
using Hypepool.Common.Pools;
using Hypepool.Core.Internals.Factories.Server;
using Hypepool.Core.Shares;
using Hypepool.Core.Stratum;

namespace Hypepool.Core.Pools
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
