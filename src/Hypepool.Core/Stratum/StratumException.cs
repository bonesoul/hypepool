using System;
using System.Collections.Generic;
using System.Text;

namespace Hypepool.Core.Stratum
{
    /// <summary>
    /// Stratum exeption.
    /// </summary>
    public class StratumException : Exception
    {
        /// <summary>
        /// Error code.
        /// </summary>
        public StratumError Code { get; set; }

        public StratumException(StratumError code, string message) 
            : base(message)
        {
            Code = code;
        }
    }
}
