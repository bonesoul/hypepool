using Hypepool.Core.Blockchain.Monero;
using Hypepool.Core.Core;
using Hypepool.Core.Internals.Factories.Pool;
using SimpleInjector;

namespace Hypepool.Core.Internals.Registries
{
    public class FactoryRegistry : IRegistry
    {
        private readonly Container _container;

        public FactoryRegistry(Container container)
        {
            _container = container;
        }

        public void Run()
        {
            _container.RegisterSingleton<IPoolFactory, PoolFactory>();
        }
    }
}
