using DryIoc;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.DependencyManagement;
using mobSocial.Data;
using mobSocial.Data.Database;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.Plugins.OAuth.Entity.OAuth;
using mobSocial.Plugins.OAuth.Services;
using mobSocial.Services.Authentication;
using mobSocial.WebApi.Configuration.Database;

namespace mobSocial.Plugins.OAuth
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string ContextServiceKey = "mobSocial.Plugins.OAuth.DbContext";

        public void RegisterDependencies(Container container)
        {
            container.Register<IClientService, ClientService>();
            container.Register<IAppTokenService, AppTokenService>();

            container.Register<IDatabaseContext>(made: Made.Of(() => DatabaseContextManager.GetDatabaseContext<OAuthDbContext>()), serviceKey: ContextServiceKey, reuse: Reuse.InWebRequest);

            container.RegisterDelegate<IDataRepository<OAuthClient>>(
                resolver => new EntityRepository<OAuthClient>(ContextServiceKey),
                ifAlreadyRegistered: IfAlreadyRegistered.Replace,
                reuse: Reuse.Singleton);

            container.RegisterDelegate<IDataRepository<AppToken>>(
               resolver => new EntityRepository<AppToken>(ContextServiceKey),
               ifAlreadyRegistered: IfAlreadyRegistered.Replace,
               reuse: Reuse.Singleton);

            //override authentication service
            container.Register<IAuthenticationService, OAuthAuthenticationService>(
                ifAlreadyRegistered: IfAlreadyRegistered.Replace, reuse: Reuse.InWebRequest);
        }

        public int Priority => 0;
    }
}