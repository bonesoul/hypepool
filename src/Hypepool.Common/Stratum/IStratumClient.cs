using System.Net;

namespace Hypepool.Common.Stratum
{
    public interface IStratumClient
    {
        IPEndPoint LocalEndpoint { get; }

        IPEndPoint RemoteEndpoint { get; }

        string ConnectionId { get; }
    }
}
