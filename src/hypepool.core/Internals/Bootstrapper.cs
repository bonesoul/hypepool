using System;
using System.Collections.Generic;
using System.Text;
using SimpleInjector;

namespace hypepool.core.Internals
{
    public class Bootstrapper : IBootstrapper
    {
        public Container Container { get; }

        public Bootstrapper()
        {
            Container = new Container();
        }
    }
}
