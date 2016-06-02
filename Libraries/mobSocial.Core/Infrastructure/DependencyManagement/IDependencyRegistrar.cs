using DryIoc;

namespace mobSocial.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        void RegisterDependencies(Container container);

        int Priority { get; }
    }
}