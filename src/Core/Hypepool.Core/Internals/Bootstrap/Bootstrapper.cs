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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hypepool.Core.Internals.Registries;
using SimpleInjector;

namespace Hypepool.Core.Internals.Bootstrap
{
    public class Bootstrapper : IBootstrapper
    {
        public Container Container { get; }

        private readonly List<IRegistry> _registeries;

        public Bootstrapper()
        {
            Container = new Container();

            _registeries = new List<IRegistry>
            {
                new CoreRegistery(Container),
                new FactoryRegistry(Container),
                new PoolRegistry(Container),
                new ServerRegistry(Container)
            };
        }

        public void Run()
        {
            HandleRegisteries(); // handle main registeries.
            HandlePackages(); // handle packages.
        }

        private void HandleRegisteries()
        {
            // process the registries.
            foreach (var registry in _registeries)
            {
                registry.Run();
            }
        }

        private void HandlePackages()
        {
            // process packages from main executable.
            var assemblies = new List<Assembly>
            {
                Assembly.GetEntryAssembly()
            };

            // process packages from dll assemblies.
            var dllAssemblies =
                from file in new DirectoryInfo(".").GetFiles()
                where file.Extension.ToLower() == ".dll"
                select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));

            assemblies.AddRange(dllAssemblies);

            Container.RegisterPackages(assemblies); // register the packages.
        }
    }
}
