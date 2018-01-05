using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Factories.Server;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Mining.Context;
using Hypepool.Common.Pools;
using Hypepool.Common.Stratum;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroPool : PoolBase<MoneroShare>
    {

        public MoneroPool(IServerFactory serverFactory)
            : base(serverFactory)
        {
            _logger = Log.ForContext<MoneroPool>();
        }

        protected override WorkerContext CreateClientContext()
        {
            return new MoneroWorkerContext();
        }

        public override Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest)
        {
            var request = timeStampedRequest.Value;
            var context = client.GetContextAs<MoneroWorkerContext>();

            return null;
        }
    }
}