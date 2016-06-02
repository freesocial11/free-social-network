using DryIoc;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.DependencyManagement;
using mobSocial.Data;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.Plugins.OAuth.Entity.OAuth;
using mobSocial.Plugins.OAuth.Services;
using mobSocial.Services.Authentication;
using mobSocial.WebApi.Configuration.Database;

namespace mobSocial.Plugins.OAuth
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDependencies(Container container)
        {
            container.Register<IClientService, ClientService>();
            container.Register<IAppTokenService, AppTokenService>();

            container.RegisterDelegate<IDataRepository<OAuthClient>>(
                resolver => new EntityRepository<OAuthClient>(DatabaseContextManager.GetDatabaseContext<OAuthDbContext>()),
                ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            container.RegisterDelegate<IDataRepository<AppToken>>(
               resolver => new EntityRepository<AppToken>(DatabaseContextManager.GetDatabaseContext<OAuthDbContext>()),
               ifAlreadyRegistered: IfAlreadyRegistered.Replace);

            //override authentication service
            container.Register<IAuthenticationService, OAuthAuthenticationService>(
                ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        }

        public int Priority => 0;
    }
}