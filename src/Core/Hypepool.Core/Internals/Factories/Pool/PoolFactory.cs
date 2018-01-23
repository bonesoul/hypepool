using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hypepool.Common.Pools;
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

        public IPool GetPool(string name)
        {
            var registrations = _container.GetAllInstances<IPool>();
            return registrations.First();
        }

        public IPoolContext GetPoolContext()
        {
            return _container.GetInstance<IPoolContext>();
        }
    }
}
