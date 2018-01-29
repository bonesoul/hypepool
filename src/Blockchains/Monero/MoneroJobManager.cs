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

using System.Security.Cryptography;
using System.Threading.Tasks;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Monero.Daemon.Requests;
using Hypepool.Monero.Daemon.Responses;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroJobManager : JobManagerBase<MoneroJob>
    {
        private readonly byte[] instanceId;
        private readonly JobCounter _jobCounter;

        public MoneroJobManager()
        {
            _logger = Log.ForContext<MoneroJobManager>();

            _jobCounter = new JobCounter();

            using (var rng = RandomNumberGenerator.Create())
            {
                instanceId = new byte[MoneroConstants.InstanceIdSize];
                rng.GetNonZeroBytes(instanceId);
            }
        }

        public override async Task<MoneroJob> Start()
        {
            _logger.Information($"starting job manager..");

            var newJob = await UpdateJob();
            return newJob ? CurrentJob : null;
        }

        protected override async Task<bool> UpdateJob()
        {
            // cook our getblocktemplate() request.
            var request = new GetBlockTemplateRequest
            {
                WalletAddress = PoolContext.PoolAddress,
                ReserveSize = MoneroConstants.ReserveSize
            };

            // call getblocktemplate() from daemon.
            var response = await PoolContext.MiningDaemon.ExecuteCommandAsync<GetBlockTemplateResponse>(MoneroRpcCommands.GetBlockTemplate, request);

            // make sure we are free of errors.
            if (response.Error != null)
            {
                _logger.Error($"New job creation failed as daemon responded with [{response.Error.Code}] {response.Error.Message}");
            }

            // get the response.
            var blockTemplate = response.Response;

            // check if we really got a new block.
            var gotNewBlock = (CurrentJob == null || CurrentJob.BlockTemplate.Height < blockTemplate.Height);

            // if we got a new block there, create a new job for it then.
            if (gotNewBlock)
            {
                var jobId = _jobCounter.GetNext(); // create a new job id.
                var job = new MoneroJob(blockTemplate, instanceId, jobId); // cook the job.
                CurrentJob = job; // set the current job.
            }

            return gotNewBlock;
        }
    }
}
