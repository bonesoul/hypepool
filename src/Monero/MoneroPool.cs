using Hypepool.Common.Factories.Server;
using Hypepool.Common.Pools;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {
        private readonly ILogger _logger;

        public MoneroPool(IServerFactory serverFactory)
            : base(serverFactory)
        {
            _logger = Log.ForContext<MoneroPool>();
        }

        public override void Initialize()
        {
            _logger.Information("Initializing pool..");

            StratumServer.Initialize();
        }
    }
}