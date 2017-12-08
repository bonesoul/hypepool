using SimpleInjector;

namespace hypepool.core.Internals
{
    public interface IBootstrapper
    {
        Container Container { get; }
    }
}
