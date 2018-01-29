#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion

using Hypepool.Common.Daemon;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;

namespace Hypepool.Common.Pools
{
    public abstract class PoolContext<TJob> : IPoolContext<TJob> where TJob : IJob
    {

        public IDaemonClient MiningDaemon { get; private set; }

        public IDaemonClient WalletDaemon { get; private set; }

        public JobManagerBase<TJob> JobManager { get; private set; }

        public IStratumServer StratumServer { get; private set; }

        // todo: move this.
        public abstract string PoolAddress { get; }

        public void Configure(IDaemonClient miningDaemon, IDaemonClient walletDaemon, JobManagerBase<TJob> jobManager, IStratumServer stratumServer)
        {
            MiningDaemon = miningDaemon;
            WalletDaemon = walletDaemon;
            JobManager = jobManager;
            StratumServer = stratumServer;
        }
    }
}
