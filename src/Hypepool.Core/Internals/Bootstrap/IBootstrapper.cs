using SimpleInjector;

namespace Hypepool.Core.Internals.Bootstrap
{
    public interface IBootstrapper
    {
        Container Container { get; }
    }
}
