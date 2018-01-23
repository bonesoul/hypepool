using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Daemon;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public class PoolContext : IPoolContext
    {
        public IStratumServer StratumServer { get; private set; }

        public IJobManager JobManager { get; private set; }

        public IDaemonClient DaemonClient { get; private set; }

        public void Attach(IDaemonClient daemonClient, IJobManager jobManager, IStratumServer stratumServer)
        {
            DaemonClient = daemonClient;
            JobManager = jobManager;
            StratumServer = stratumServer;
        }
    }
}
