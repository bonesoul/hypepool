using System;
using System.Collections.Generic;
using System.Text;
using Hypepool.Common.Utils.Time;

namespace Hypepool.Common.Mining.Context
{
    public class WorkerContext
    {
        /// <summary>
        /// Last activity by worker.
        /// </summary>
        public DateTime LastActivity { get; set; }

        public bool IsAuthorized { get; set; } = false;

        public bool IsSubscribed { get; set; } = false;

        /// <summary>
        /// Difficulty assigned to this worker, either static or updated through VarDiffManager
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// Previous difficulty assigned to this worker
        /// </summary>
        public double? PreviousDifficulty { get; set; }

        /// <summary>
        /// UserAgent reported by Stratum
        /// </summary>
        public string UserAgent { get; set; }

        public void Initialize(double difficulty, IMasterClock clock)
        {
            Difficulty = difficulty;
            LastActivity = clock.Now;
        }
    }
}
