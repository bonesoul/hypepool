#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion

using System.Collections.Generic;
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
