using System;

namespace Hypepool.Common.Utils.Time
{
    public interface IMasterClock
    {
        DateTime Now { get; }
    }
}
