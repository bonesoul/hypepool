using System.Collections.Generic;
using Hypepool.Common;
using Hypepool.Common.Pools;

namespace Hypepool.Core.Core
{
    public interface IEngine
    {
        IReadOnlyList<IPool> Pools { get; }

        void Initialize();
    }
}
