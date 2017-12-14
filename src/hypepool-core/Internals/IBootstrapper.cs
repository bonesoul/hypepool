using SimpleInjector;

namespace Hypepool.Core.Internals
{
    public interface IBootstrapper
    {
        Container Container { get; }
    }
}
