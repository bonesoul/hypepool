using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hypepool.Common.Pools;

namespace Hypepool.Common.Mining.Jobs
{
    public interface IJobManager
    {
        void Initialize(IPoolContext poolContext);

        void Start();
    }
}
