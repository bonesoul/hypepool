using Hypepool.Common.Stratum;

namespace Hypepool.Common.Factories.Server
{
    public interface IServerFactory
    {
        IStratumServer GetStratumServer();
    }
}
