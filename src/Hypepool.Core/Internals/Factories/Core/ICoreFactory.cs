﻿using Hypepool.Core.Core;

namespace Hypepool.Core.Internals.Factories.Core
{
    public interface ICoreFactory
    {
        IEngine GetEngine();
    }
}
