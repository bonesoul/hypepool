using SimpleInjector;

namespace Hypepool.Core.Internals
{
    public class Bootstrapper : IBootstrapper
    {
        public Container Container { get; }

        public Bootstrapper()
        {
            Container = new Container();
        }

        public void Run()
        {

        }
    }
}
