using System.Collections.Generic;
using Hypepool.Common.Pools;
using NetUV.Core.Handles;

namespace Hypepool.Common.Stratum
{
    public interface IStratumServer
    {
        IReadOnlyDictionary<int, Tcp> Ports { get; }

        IReadOnlyDictionary<string, IStratumClient> Clients { get; }

        void Start(IPool pool);

        void Stop();
    }
}
