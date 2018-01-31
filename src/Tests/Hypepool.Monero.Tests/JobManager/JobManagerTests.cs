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
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Hypepool.Common.Daemon;
using Hypepool.Common.Pools;
using Hypepool.Monero.Daemon.Requests;
using Hypepool.Monero.Daemon.Responses;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;

namespace Hypepool.Monero.Tests.JobManager
{
    public class JobManagerTests
    {
        private readonly MoneroPoolContext _poolContext;
        private readonly IDaemonClient _daemon;


        public JobManagerTests()
        {
            // mock objects.
            _daemon = Substitute.For<IDaemonClient>();

            var jobManager = new MoneroJobManager();
            _poolContext = new MoneroPoolContext();

            _poolContext.Configure(_daemon, _daemon, jobManager, null);
            jobManager.Configure(_poolContext);
        }

        [Fact]
        public async void ShouldGetANewJobOnDifferentBlocktemplates()
        {
            var counter = 0;
            _daemon.ExecuteCommandAsync<GetBlockTemplateResponse>(MoneroRpcCommands.GetBlockTemplate, Arg.Any<GetBlockTemplateRequest>())
                .ReturnsForAnyArgs(x =>
                {
                    var response = counter == 0
                        ? JobManagerTestsContants.DaemonResponse1088295 // response in first call.
                        : JobManagerTestsContants.DaemonResponse1088296; // response in second call.

                    counter++;
                    return response;
                });

            await _poolContext.JobManager.Start(); // start the job manager.

            _poolContext.JobManager.JobQueue
                .Take(3)
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                }, () => 
                    
                    Console.WriteLine("Completed"));


            var test2 = _poolContext.JobManager.JobQueue.Take(2)
                .Subscribe(x =>
                {
                    var b = "b";
                });

            var a = "b";


            //_poolContext.JobManager.JobQueue.Wait(); // wait for rx to process.

            //var task = Task.Factory.StartNew(() =>
            //{
            //    _poolContext.JobManager.JobQueue.Subscribe(_ =>
            //    {
            //        var response = ((MoneroJobManager) _poolContext.JobManager).CurrentJob;

            //        if (counter == 1) // check first response.
            //        {
            //            response.BlockTemplate.Blob.ShouldBeEquivalentTo("07079ccdc1d3053b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b0000000002e3b64201ffa7b64201b3be90f18de9010258f3cdf062d631c578597ad4aa00bb6e99bbbdc816fa295286ea956c2d15e1cf2b0109d8ec8c47fb41668d69ef2ce3764a906285bbab66a127247f627b939380ddc7020800000000000000000000");
            //            response.BlockTemplate.Difficulty.ShouldBeEquivalentTo(23096);
            //            response.BlockTemplate.Height.ShouldBeEquivalentTo(1088295);
            //            response.BlockTemplate.PreviousBlockhash.ShouldBeEquivalentTo("3b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b");
            //            response.BlockTemplate.ReservedOffset.ShouldBeEquivalentTo(129);
            //            response.BlockTemplate.Status.ShouldBeEquivalentTo("OK");
            //        }
            //        else if (counter == 2) // check second response.
            //        {
            //            response.BlockTemplate.Blob.ShouldBeEquivalentTo("0707b0cfc1d305a6a0d38480cde292cbcd372835fb8d57ff8285b5334c724f610599d379c5499d0000000002e4b64201ffa8b64201ee86ece98de90102d287d0af34a8c9dc624cf4a7495f673782d9a323b1d22dec71b069af84c072672b013b54f7d869b695fd5e5fa05ddf49e971f511440b86ff42ce836283d207777442020800000000000000000000");
            //            response.BlockTemplate.Difficulty.ShouldBeEquivalentTo(23087);
            //            response.BlockTemplate.Height.ShouldBeEquivalentTo(1088296);
            //            response.BlockTemplate.PreviousBlockhash.ShouldBeEquivalentTo("a6a0d38480cde292cbcd372835fb8d57ff8285b5334c724f610599d379c5499d");
            //            response.BlockTemplate.ReservedOffset.ShouldBeEquivalentTo(129);
            //            response.BlockTemplate.Status.ShouldBeEquivalentTo("OK");
            //        }
            //    });

            //    _poolContext.JobManager.JobQueue.Wait(); // wait for rx to process.
            //});

            //task.Wait(1000);
        }

        [Fact]
        public async void ShouldNotGetANewJobOnSameBlockTemplate()
        {
            //_daemon.ExecuteCommandAsync<GetBlockTemplateResponse>(MoneroRpcCommands.GetBlockTemplate,Arg.Any<GetBlockTemplateRequest>())
            //    .Returns(JobManagerTestsContants.DaemonResponse1088295);                       


            //// check first response.
            //var firstResponse = (MoneroJob)await _jobManager.Start();
            //firstResponse.ShouldBeEquivalentTo(_jobManager.CurrentJob);
            //firstResponse.BlockTemplate.Blob.ShouldBeEquivalentTo("07079ccdc1d3053b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b0000000002e3b64201ffa7b64201b3be90f18de9010258f3cdf062d631c578597ad4aa00bb6e99bbbdc816fa295286ea956c2d15e1cf2b0109d8ec8c47fb41668d69ef2ce3764a906285bbab66a127247f627b939380ddc7020800000000000000000000");
            //firstResponse.BlockTemplate.Difficulty.ShouldBeEquivalentTo(23096);
            //firstResponse.BlockTemplate.Height.ShouldBeEquivalentTo(1088295);
            //firstResponse.BlockTemplate.PreviousBlockhash.ShouldBeEquivalentTo("3b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b");
            //firstResponse.BlockTemplate.ReservedOffset.ShouldBeEquivalentTo(129);
            //firstResponse.BlockTemplate.Status.ShouldBeEquivalentTo("OK");

            //// check second response.
            //var secondResponse = (MoneroJob)await _jobManager.Start();
            //secondResponse.Should().BeNull();
        }
    }

    public static class JobManagerTestsContants
    {
        // cook actual blocktemplate responses.
        public static DaemonResponse<GetBlockTemplateResponse> DaemonResponse1088295 = new DaemonResponse<GetBlockTemplateResponse>
        {
            Error = null,
            Response = new GetBlockTemplateResponse
            {
                Blob = "07079ccdc1d3053b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b0000000002e3b64201ffa7b64201b3be90f18de9010258f3cdf062d631c578597ad4aa00bb6e99bbbdc816fa295286ea956c2d15e1cf2b0109d8ec8c47fb41668d69ef2ce3764a906285bbab66a127247f627b939380ddc7020800000000000000000000",
                Difficulty = 23096,
                Height = 1088295,
                PreviousBlockhash = "3b4aff4fdac0424b58a69234796f1e572eea2650cb4a6c19aa03c17308d73d5b",
                ReservedOffset = 129,
                Status = "OK"
            }
        };

        public static DaemonResponse<GetBlockTemplateResponse> DaemonResponse1088296 = new DaemonResponse<GetBlockTemplateResponse>
        {
            Error = null,
            Response = new GetBlockTemplateResponse
            {
                Blob = "0707b0cfc1d305a6a0d38480cde292cbcd372835fb8d57ff8285b5334c724f610599d379c5499d0000000002e4b64201ffa8b64201ee86ece98de90102d287d0af34a8c9dc624cf4a7495f673782d9a323b1d22dec71b069af84c072672b013b54f7d869b695fd5e5fa05ddf49e971f511440b86ff42ce836283d207777442020800000000000000000000",
                Difficulty = 23087,
                Height = 1088296,
                PreviousBlockhash = "a6a0d38480cde292cbcd372835fb8d57ff8285b5334c724f610599d379c5499d",
                ReservedOffset = 129,
                Status = "OK"
            }
        };
    }
}


//var scheduler = new TestScheduler();
//var results = scheduler.CreateObserver<MoneroJob>();

//_poolContext.JobManager.JobQueue.Select(unit => ((MoneroJobManager) _poolContext.JobManager).CurrentJob).Subscribe(results);

//var testObserver = scheduler.Start(x => _
//{

//_poolContext.JobManager.JobQueue.Select(unit => ((MoneroJobManager)_poolContext.JobManager).CurrentJob).Subscribe(results);
//}, 0,
//TimeSpan.FromSeconds(1).Ticks,
//TimeSpan.FromSeconds(5).Ticks);


//var test = results.Messages;