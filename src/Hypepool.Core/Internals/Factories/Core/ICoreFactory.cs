using Hypepool.Core.Core;
using Hypepool.Core.Internals.Logging;

namespace Hypepool.Core.Internals.Factories.Core
{
    public interface ICoreFactory
    {
        /// <summary>
        /// Returns engine instance.
        /// </summary>
        /// <returns><see cref="IEngine"/></returns>
        IEngine GetEngine();

        /// <summary>
        /// Returns log manager.
        /// </summary>
        /// <returns><see cref="ILogManager"/></returns>
        ILogManager GetLogManager();
    }
}
