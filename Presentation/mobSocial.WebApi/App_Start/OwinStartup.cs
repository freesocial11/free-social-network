using System.Web.Http;
using DryIoc.Owin;
using DryIoc.WebApi.Owin;
using mobSocial.Core.Infrastructure.AppEngine;
using Microsoft.Owin;
using Owin;
using mobSocial.WebApi;
using mobSocial.WebApi.Configuration.Middlewares;
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

            app.UseDryIocOwinMiddleware(mobSocialEngine.ActiveEngine.IocContainer);
            
            app.UseMobAuthentication();

            app.UsePictureSizeRegistrar();

            //route registrations & other configurations
            WebApiConfig.Register(config);

#if DEBUG
            app.UseErrorPage(new ErrorPageOptions());
#endif

            //run owin startup configurations from plugins
            OwinStartupManager.RunAllOwinConfigurations(app);

            //webapi, last one always 
            app.UseWebApi(config);

            //di handler
            //app.UseDryIocWebApi(config);
        }
    }
}