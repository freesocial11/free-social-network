using System.Web.Http;
using DryIoc.Owin;
using DryIoc.WebApi.Owin;
using mobSocial.Core.Infrastructure.AppEngine;
using Microsoft.Owin;
using Owin;
using mobSocial.WebApi;
using mobSocial.WebApi.Configuration.Middlewares;
using mobSocial.WebApi.Configuration.SignalR.Providers;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Diagnostics;

[assembly: OwinStartup(typeof(OwinStartup))]
namespace mobSocial.WebApi
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            //new configuration for owin
            var config = new HttpConfiguration();

            app.UseInstallationVerifier();

            //run owin startup configurations from plugins
            OwinStartupManager.RunAllOwinConfigurations(app);

            //route registrations & other configurations
            WebApiConfig.Register(config);

            app.UseDryIocOwinMiddleware(mobSocialEngine.ActiveEngine.IocContainer);

            app.UsePictureSizeRegistrar();

            app.UseMobAuthentication();

#if DEBUG
            app.UseErrorPage(new ErrorPageOptions());
#endif
            var userIdProvider = new SignalRUserIdProvider();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);
            app.MapSignalR();

            //webapi, last one always 
            app.UseWebApi(config);
           
        }
    }
}