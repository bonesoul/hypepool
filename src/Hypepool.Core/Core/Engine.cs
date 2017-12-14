using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hypepool.Common;
using Hypepool.Core.Internals.Factories.Pool;

namespace Hypepool.Core.Core
{
    public class Engine : IEngine
    {
        public IReadOnlyList<IPool> Pools { get; }

        private readonly IPoolFactory _poolFactory;
        private readonly IList<IPool> _pools;

        public Engine(IPoolFactory poolFactory)
        {
            _poolFactory = poolFactory;

            _pools = new List<IPool>();
            Pools = new ReadOnlyCollection<IPool>(_pools);
        }

        public void Initialize()
        {
            _pools.Add(_poolFactory.GetMoneroPool());
        }
    }
}
