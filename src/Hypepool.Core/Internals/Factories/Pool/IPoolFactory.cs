using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Pools;

namespace Hypepool.Core.Internals.Factories.Pool
{
    public interface IPoolFactory
    {
        IPool GetPool(string name);
    }
}
