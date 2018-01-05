using System;

namespace Hypepool.Common.Utils.Time
{
    public class StandardClock : IMasterClock
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
