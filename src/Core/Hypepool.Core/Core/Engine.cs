﻿#region license
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hypepool.Common.Pools;
using Hypepool.Core.Internals.Factories.Pool;
using Hypepool.Core.Utils.Logging;
using Serilog;

namespace Hypepool.Core.Core
{
    public class Engine : IEngine
    {
        public IReadOnlyList<IPool> Pools { get; }

        private readonly IPoolFactory _poolFactory;
        private readonly IList<IPool> _pools;
        private readonly ILogger _logger;

        public Engine(ILogManager logManager, IPoolFactory poolFactory)
        {
            _logger = Log.ForContext<Engine>();
            _poolFactory = poolFactory;

            _pools = new List<IPool>();
            Pools = new ReadOnlyCollection<IPool>(_pools);
        }

        public async Task Start()
        {
            _logger.Information("starting engine..");

            _pools.Add(_poolFactory.GetPool("Monero"));

            foreach (var pool in _pools)
            {
                await pool.Initialize();
            }

            foreach (var pool in _pools)
            {
                await pool.Start();
            }
        }
    }
}
