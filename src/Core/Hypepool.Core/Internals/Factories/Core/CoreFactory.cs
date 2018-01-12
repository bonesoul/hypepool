using Hypepool.Core.Core;
using Hypepool.Core.Internals.Logging;
using SimpleInjector;

namespace Hypepool.Core.Internals.Factories.Core
{
    public class CoreFactory: ICoreFactory
    {
        private readonly Container _container;

        public CoreFactory(Container container)
        {
            _container = container;
        }

        public IEngine GetEngine()
        {
            return _container.GetInstance<IEngine>();
        }

        public ILogManager GetLogManager()
        {
            return _container.GetInstance<ILogManager>();
        }
    }
}
