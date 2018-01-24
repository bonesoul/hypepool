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
using Hypepool.Common.Utils.Time;

namespace Hypepool.Common.Mining.Context
{
    public class WorkerContext
    {
        /// <summary>
        /// Last activity by worker.
        /// </summary>
        public DateTime LastActivity { get; set; }

        public bool IsAuthorized { get; set; } = false;

        public bool IsSubscribed { get; set; } = false;

        /// <summary>
        /// Difficulty assigned to this worker, either static or updated through VarDiffManager
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// Previous difficulty assigned to this worker
        /// </summary>
        public double? PreviousDifficulty { get; set; }

        /// <summary>
        /// UserAgent reported by Stratum
        /// </summary>
        public string UserAgent { get; set; }

        public void Initialize(double difficulty, IMasterClock clock)
        {
            Difficulty = difficulty;
            LastActivity = clock.Now;
        }
    }
}
