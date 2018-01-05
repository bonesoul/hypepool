using Hypepool.Core.Core;
using Hypepool.Core.Internals.Logging;
using SimpleInjector;

namespace Hypepool.Core.Internals.Registries
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
            _container.RegisterSingleton<ILogManager, LogManager>();
        }
    }
}
