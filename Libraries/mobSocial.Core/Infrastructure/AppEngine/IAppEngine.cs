using DryIoc;

namespace mobSocial.Core.Infrastructure.AppEngine
{
    public interface IAppEngine
    {
        Container IocContainer { get; }

        T Resolve<T>(bool returnDefaultIfNotResolved = false) where T : class;

        T RegisterAndResolve<T>(object instance, bool instantiateIfNull = true) where T : class;

        void Start(bool testMode = false);
    }
}