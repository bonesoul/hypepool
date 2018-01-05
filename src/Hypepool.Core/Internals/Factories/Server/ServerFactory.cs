using Hypepool.Common.Factories.Server;
using Hypepool.Common.Stratum;
using Hypepool.Core.Stratum;
using SimpleInjector;

namespace Hypepool.Core.Internals.Factories.Server
{
    public class ServerFactory : IServerFactory
    {
        private readonly Container _container;

        public ServerFactory(Container container)
        {
            _container = container;
        }

        public IStratumServer GetStratumServer()
        {
            return _container.GetInstance<IStratumServer>();
        }
    }
}
