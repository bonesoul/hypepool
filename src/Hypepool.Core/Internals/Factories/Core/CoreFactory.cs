using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Core.Blockchain.Monero;
using Hypepool.Core.Core;
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
    }
}
