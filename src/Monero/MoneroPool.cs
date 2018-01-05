using Hypepool.Common.Factories.Server;
using Hypepool.Common.Pools;

namespace Hypepool.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {
        public MoneroPool(IServerFactory serverFactory)
            : base(serverFactory)
        {

        }

        public override void Initialize()
        {
            StratumServer.Initialize();
        }
    }
}