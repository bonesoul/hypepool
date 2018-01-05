using System;
using System.Collections.Generic;
using System.Text;

namespace Hypepool.Core.Utils.Time
{
    public class StandardClock : IMasterClock
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
