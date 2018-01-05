using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.JsonRpc;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public interface IPool
    {
        IStratumServer StratumServer { get; }

        Task StartAsync();

        void OnConnect(IStratumClient client);

        void OnDisconnect(string subscriptionId);

        Task OnRequestAsync(IStratumClient client, Timestamped<JsonRpcRequest> timeStampedRequest);
    }
}
