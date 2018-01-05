using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Mining.Context;

namespace Hypepool.Monero
{
    public class MoneroWorkerContext : WorkerContext
    {
        public string MinerName { get; set; }

        public string WorkerName { get; set; }

        /// <summary>
        /// Payment ID is an arbitrary and optional transaction attachment that consists of 32 bytes (64 hexadecimal characters) 
        /// or 8 bytes (in the case of integrated addresses).
        /// https://getmonero.org/resources/moneropedia/paymentid.html
        /// </summary>
        public string PaymentId { get; set; }
    }
}
