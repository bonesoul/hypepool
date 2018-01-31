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

using System;
using System.Reactive;
using System.Threading.Tasks;
using Hypepool.Common.Pools;
using Serilog;

namespace Hypepool.Common.Mining.Jobs
{
    public abstract class JobManagerBase<TJob> : IJobManager where TJob : IJob
    {
        protected IPoolContext PoolContext { get; private set; }

        public TJob CurrentJob { get; protected set; }

        public IObservable<Unit> JobQueue { get; protected set; }

        /// <summary>
        /// Starts the job manager.
        /// </summary>
        public abstract Task Start();

        /// <summary>
        /// Queries the network and updates the job if needed.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> UpdateJob();

        protected ILogger _logger;

        public void Configure(IPoolContext poolContext)
        {
            PoolContext = poolContext;
        }
    }
}
