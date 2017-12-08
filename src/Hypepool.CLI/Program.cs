using Hypepool.Core.Internals;

namespace Hypepool.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper(); // IoC kernel bootstrapper.
            bootstrapper.Run();
        }
    }
}
