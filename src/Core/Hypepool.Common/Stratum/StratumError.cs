namespace Hypepool.Common.Stratum
{
    /// <summary>
    /// Stratum errors.
    /// </summary>
    public enum StratumError
    {
        MinusOne = -1,
        Other = 20,
        JobNotFound = 21, // stale
        DuplicateShare = 22,
        LowDifficultyShare = 23,
        UnauthorizedWorker = 24,
        NotSubscribed = 25
    }
}
