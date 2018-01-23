using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hypepool.Common.Pools;

namespace Hypepool.Common.Mining.Jobs
{
    public abstract class JobManagerBase<TJob> : IJobManager
    {
        protected IPoolContext PoolContext { get; private set; }

        public void Initialize(IPoolContext poolContext)
        {
            PoolContext = poolContext;
        }

        public abstract void Start();
    }
}
