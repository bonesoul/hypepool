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

    public static class MoneroRpcCommands
    {
        public const string GetInfo = "get_info";
        public const string GetBlockTemplate = "getblocktemplate";
        public const string SubmitBlock = "submitblock";
        public const string GetBlockHeaderByHash = "getblockheaderbyhash";
        public const string GetBlockHeaderByHeight = "getblockheaderbyheight";
    }

    public static class MoneroWalletCommands
    {
        public const string GetBalance = "getbalance";
        public const string GetAddress = "getaddress";
        public const string Transfer = "transfer";
        public const string TransferSplit = "transfer_split";
        public const string GetTransfers = "get_transfers";
        public const string SplitIntegratedAddress = "split_integrated_address";
    }
}
