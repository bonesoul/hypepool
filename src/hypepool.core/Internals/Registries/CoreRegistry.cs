using SimpleInjector;
using hypepool.core.Core;

namespace hypepool.core.Internals.Registries
{
    public class CoreRegistery : IRegistry
    {
        private readonly Container _container;

        public CoreRegistery(Container container)
        {
            _container = container;
        }

        public void Run()
        {
            _container.RegisterSingleton<IEngine, Engine>();
        }
    }
}
