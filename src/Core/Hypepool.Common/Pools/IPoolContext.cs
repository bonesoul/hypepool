using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Daemon;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public interface IPoolContext
    {
        IDaemonClient DaemonClient { get; }

        IJobManager JobManager { get; }

        IStratumServer StratumServer { get; }

        void Attach(IDaemonClient daemonClient, IJobManager jobManager, IStratumServer stratumServer);
    }
}
