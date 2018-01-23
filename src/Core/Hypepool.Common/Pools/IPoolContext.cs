using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public interface IPoolContext
    {
        IStratumServer StratumServer { get; }

        IJobManager JobManager { get; }

        void Attach(IStratumServer stratumServer, IJobManager jobManager);
    }
}
