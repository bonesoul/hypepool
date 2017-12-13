using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hypepool.Core.Internals.Registries;
using SimpleInjector;

namespace Hypepool.Core.Internals
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
                new CoreRegistery(Container)
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
