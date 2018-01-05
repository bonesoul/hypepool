using Hypepool.Common.Factories.Server;
using Hypepool.Core.Internals.Factories.Core;
using Hypepool.Core.Internals.Factories.Pool;
using Hypepool.Core.Internals.Factories.Server;
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
            _container.RegisterSingleton<ICoreFactory, CoreFactory>();
            _container.RegisterSingleton<IServerFactory, ServerFactory>();
        }
    }
}
