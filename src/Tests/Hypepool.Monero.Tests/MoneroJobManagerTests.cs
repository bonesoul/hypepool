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
using Hypepool.Common.Stratum;
using Hypepool.Monero;
using NSubstitute;
using Xunit;

namespace Hypepool.Core.Tests
{
    public class MoneroJobManagerTests
    {
        private MoneroJobManager _jobManager;

        public MoneroJobManagerTests()
        {
            _jobManager = new MoneroJobManager();

            var miningDaemon = Substitute.For<IDaemonClient>();
            var walletDaemon = Substitute.For<IDaemonClient>();
            var stratumServer = Substitute.For<IStratumServer>();           

            var poolContext = new MoneroPoolContext();
            poolContext.Configure(miningDaemon, walletDaemon, _jobManager, stratumServer);

            _jobManager.Configure(poolContext);
        }

        [Fact]
        public void ShouldGetANewJob()
        {

        }
    }
}
