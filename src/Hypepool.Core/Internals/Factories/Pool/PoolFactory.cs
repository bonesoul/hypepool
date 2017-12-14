using Hypepool.Core.Blockchain.Monero;
using SimpleInjector;

namespace Hypepool.Core.Internals.Factories.Pool
{
    public class PoolFactory : IPoolFactory
    {
        private readonly Container _container;

        public PoolFactory(Container container)
        {
            _container = container;
        }

        public MoneroPool GetMoneroPool()
        {
            return _container.GetInstance<MoneroPool>();
        }
    }
}
