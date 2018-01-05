using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Coins;

namespace Hypepool.Monero
{
    public class MoneroConstants
    {
        public static readonly Dictionary<CoinType, int> AddressLength = new Dictionary<CoinType, int>
        {
            { CoinType.XMR, 95 },
            { CoinType.ETN, 98 },
            { CoinType.AEON, 97 },
        };
    }
}
