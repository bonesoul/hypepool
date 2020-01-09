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

using System.Reflection;
using FluentAssertions;
using Hypepool.Common.Mining.Jobs;
using Xunit;

namespace Hypepool.Common.Tests.Mining.Jobs
{
    public class JobCounterTests
    {
        private readonly JobCounter _jobCounter;
        public JobCounterTests()
        {
            _jobCounter = new JobCounter();
        }

        /// <summary>
        /// Verifies GetNext().
        /// </summary>
        [Fact]
        public void ShouldBeIncremented()
        {
            _jobCounter.GetNext().Should().Be(1);
            _jobCounter.GetNext().Should().Be(2);
        }

        [Fact]
        public void ShouldResetBackAfterMaxValue()
        {
            // get access for private _last field.
            var last = _jobCounter.GetType().GetField("_last",
                BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // set it to int.MaxValue - 1
            last.SetValue(_jobCounter, int.MaxValue - 1);
            last.GetValue(_jobCounter).Should().Be(int.MaxValue - 1);

            // GetNext() and should be int.MaxValue now.
            _jobCounter.GetNext().Should().Be(int.MaxValue);

            // another GetNext() and should reset back to 1 now.
            _jobCounter.GetNext().Should().Be(1);
        }
    }
}
