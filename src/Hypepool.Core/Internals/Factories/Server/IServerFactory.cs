using Hypepool.Common.Stratum;
using Hypepool.Core.Stratum;

namespace Hypepool.Core.Internals.Factories.Server
{
    public interface IServerFactory
    {
        IStratumServer GetStratumServer();
    }
}
