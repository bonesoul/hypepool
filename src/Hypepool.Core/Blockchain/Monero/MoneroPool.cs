using Hypepool.Core.Internals.Factories.Server;
using Hypepool.Core.Pools;

namespace Hypepool.Core.Blockchain.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {
        public MoneroPool(IServerFactory serverFactory)
            :base(serverFactory)
        {

        }

        public override void Initialize()
        {

        }
    }
}
