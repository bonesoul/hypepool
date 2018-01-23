using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hypepool.Common;
using Hypepool.Common.Pools;
using Hypepool.Core.Internals.Factories.Pool;
using Hypepool.Core.Internals.Logging;
using Serilog;

namespace Hypepool.Core.Core
{
    public class Engine : IEngine
    {
        public IReadOnlyList<IPool> Pools { get; }

        private readonly IPoolFactory _poolFactory;
        private readonly IList<IPool> _pools;
        private readonly ILogger _logger;

        public Engine(ILogManager logManager, IPoolFactory poolFactory)
        {
            _logger = Log.ForContext<Engine>();
            _poolFactory = poolFactory;

            _pools = new List<IPool>();
            Pools = new ReadOnlyCollection<IPool>(_pools);

            _logger.Information("Initialized engine..");
        }

        public void Initialize()
        {
            _pools.Add(_poolFactory.GetPool("Monero"));

            foreach (var pool in _pools)
            {
                pool.Initialize();
            }
        }

        public void Start()
        {
            foreach (var pool in _pools)
            {
                pool.Start();
            }
        }
    }
}
