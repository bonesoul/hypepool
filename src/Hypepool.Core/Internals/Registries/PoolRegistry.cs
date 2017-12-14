using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Core.Blockchain.Monero;
using SimpleInjector;

namespace Hypepool.Core.Internals.Registries
{
    public class PoolRegistry : IRegistry
    {
        private readonly Container _container;

        public PoolRegistry(Container container)
        {
            _container = container;
        }

        public void Run()
        {
            _container.RegisterSingleton<MoneroPool>();
        }
    }
}
