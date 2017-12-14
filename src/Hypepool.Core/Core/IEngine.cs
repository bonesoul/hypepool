using System.Collections.Generic;
using Hypepool.Common;

namespace Hypepool.Core.Core
{
    public interface IEngine
    {
        IReadOnlyList<IPool> Pools { get; }

        void Initialize();
    }
}
