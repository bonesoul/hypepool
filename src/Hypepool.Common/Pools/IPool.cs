using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public interface IPool
    {
        IStratumServer StratumServer { get; }

        void Initialize();
    }
}
