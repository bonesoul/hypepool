using System;
using Hypepool.Core.Core;
using Hypepool.Core.Internals;
using Hypepool.Core.Internals.Bootstrap;
using Hypepool.Core.Internals.Factories.Core;

namespace Hypepool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run(); // run bootstrapper.
            bootstrapper.Container.Verify(); // verify container

            var coreFactory = bootstrapper.Container.GetInstance<ICoreFactory>(); // get core object factory.
            var engine = coreFactory.GetEngine(); // get engine.

            engine.Initialize();

            while (true)
            {                
            }
        }
    }
}
