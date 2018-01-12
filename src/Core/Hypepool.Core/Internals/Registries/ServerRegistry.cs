using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Stratum;
using Hypepool.Core.Stratum;
using SimpleInjector;

namespace Hypepool.Core.Internals.Registries
{
    public class ServerRegistry : IRegistry
    {
        private readonly Container _container;

        public ServerRegistry(Container container)
        {
            _container = container;
        }

        public void Run()
        {
            _container.RegisterSingleton<IStratumServer, StratumServer>();
        }
    }
}
