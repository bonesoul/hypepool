using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Pools;
using Hypepool.Common.Stratum;
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
            base.Initialize();
        }

        public override void OnConnect(IStratumClient client)
        {
            
        }

        public override Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> request)
        {
            return null;
        }
    }
}