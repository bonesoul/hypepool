using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hypepool.Common.Mining.Jobs
{
    public interface IJobManager
    {
        Task StartAsync();
    }
}
