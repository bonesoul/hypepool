using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public class PoolContext : IPoolContext
    {
        public IStratumServer StratumServer { get; private set; }

        public IJobManager JobManager { get; private set; }

        public void Attach(IStratumServer stratumServer, IJobManager jobManager)
        {
            StratumServer = stratumServer;
            JobManager = jobManager;
        }
    }
}
