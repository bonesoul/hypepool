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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hypepool.Common.Pools;
using Hypepool.Common.Shares;
using Stashbox;

namespace Hypepool.Core.Internals.Registries
{
    public class PoolRegistry : IRegistry
    {
        private readonly StashboxContainer _container;

        public PoolRegistry(StashboxContainer container)
        {
            _container = container;
        }

        public void Run()
        {
            _container.RegisterType<IPoolContext, PoolContext>();

            var assemblies = GetAssemblies();

            foreach (var assembly in assemblies)
            {
                _container.RegisterAssembly(assembly, type => typeof(IPool).IsAssignableFrom(type));
            }

            var test = _container.CanResolve<IPool>();
        }

        private IList<Assembly> GetAssemblies()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = new List<Assembly>();

            foreach (var file in new DirectoryInfo(root).GetFiles())
            {
                if (file.Extension.ToLower() != ".dll")
                    continue;

                try
                {
                    assemblies.Add(Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return assemblies;
        }
    }
}
