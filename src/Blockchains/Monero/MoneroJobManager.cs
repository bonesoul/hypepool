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
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Hypepool.Common.Mining.Jobs;
using Hypepool.Common.Stratum;
using Hypepool.Monero.Daemon.Requests;
using Hypepool.Monero.Daemon.Responses;
using Hypepool.Monero.Stratum.Responses;
using Serilog;

namespace Hypepool.Monero
{
    public class MoneroJobManager : JobManagerBase<MoneroJob>
    {        
        private readonly byte[] instanceId;
        private readonly JobCounter _jobCounter;
        private readonly JobCounter _workerJobCounter;

        public MoneroJobManager()
        {
            _logger = Log.ForContext<MoneroJobManager>().ForContext("Pool", "XMR");

            _jobCounter = new JobCounter();
            _workerJobCounter = new JobCounter();

            using (var rng = RandomNumberGenerator.Create())
            {
                instanceId = new byte[MoneroConstants.InstanceIdSize];
                rng.GetNonZeroBytes(instanceId);
            }
        }

        public override async Task Start()
        {
            _logger.Information($"starting job manager..");

            SetupJobUpdates();
        }

        private void SetupJobUpdates()
        {
            // periodically update jobs by querying blocktemplate from daemon.

            JobQueue = Observable.Interval(TimeSpan.FromMilliseconds(1000)) // query every 1 second.
                .Select(_ => Observable.FromAsync(UpdateJob)) // check if we got a new job.
                    .Concat() // concat inner observables.
                    .Do(job => // logging.
                    {
                        if (job != null)
                            _logger.Information($"Created a new job as a new block {CurrentJob.BlockTemplate.Height} emerged in network..");
                        //else _logger.Verbose("Queried network for a new job, but found none..");
                    })
                    .Where(x => x != null) // filter out entries without an actual new job.
                    .Publish() // publish.
                    .RefCount(); // run the observer as long as we have a valid listener.
        }

        protected override async Task<MoneroJob> UpdateJob()
        {
            try
            {
                // cook our getblocktemplate() request.
                var request = new GetBlockTemplateRequest
                {
                    WalletAddress = PoolContext.PoolAddress,
                    ReserveSize = MoneroConstants.ReserveSize
                };

                // call getblocktemplate() from daemon.
                var response = await PoolContext.Daemon.ExecuteCommandAsync<GetBlockTemplateResponse>(MoneroRpcCommands.GetBlockTemplate, request);

                // make sure we are free of errors.
                if (response.Error != null)
                {
                    _logger.Error($"New job creation failed as daemon responded with [{response.Error.Code}] {response.Error.Message}");
                }

                // get the response.
                var blockTemplate = response.Response;

                // check if we really got a new blocktemplate (for a new block).
                var gotNewBlockTemplate = (CurrentJob == null || CurrentJob.BlockTemplate.Height < blockTemplate.Height);

                // if we got a new block there, create a new job for it then.
                if (gotNewBlockTemplate)
                {
                    var jobId = _jobCounter.GetNext(); // create a new job id.
                    var job = new MoneroJob(blockTemplate, instanceId, jobId); // cook the job.
                    CurrentJob = job; // set the current job.
                    return job;
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error querying for a new job.");
                return null;
            }
        }

        public MoneroJobParams CreateWorkerJob(IStratumClient client)
        {
            var context = client.GetContextAs<MoneroWorkerContext>();
            var workerJob = new MoneroWorkerJob(_workerJobCounter.GetNext(), CurrentJob.BlockTemplate.Height, context.Difficulty);
            CurrentJob.PrepareWorkerJob(workerJob);
            

            return null;
        }
    }
}
