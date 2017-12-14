using System;
using Hypepool.Core.Core;
using Hypepool.Core.Internals;

namespace Hypepool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run();

            var engine = bootstrapper.Container.GetInstance<IEngine>();
        }
    }
}
