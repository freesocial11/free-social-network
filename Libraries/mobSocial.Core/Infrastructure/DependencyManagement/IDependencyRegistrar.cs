using DryIoc;

namespace mobSocial.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        void RegisterDependencies(IContainer container);

        int Priority { get; }
    }
}