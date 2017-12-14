using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Core.Blockchain.Monero;

namespace Hypepool.Core.Internals.Factories.Pool
{
    public interface IPoolFactory
    {
        MoneroPool GetMoneroPool();
    }
}
