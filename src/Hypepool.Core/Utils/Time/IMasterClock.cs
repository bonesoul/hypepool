using System;
using System.Collections.Generic;
using System.Text;

namespace Hypepool.Core.Utils.Time
{
    public interface IMasterClock
    {
        DateTime Now { get; }
    }
}
