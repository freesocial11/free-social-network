using System.Web.Http;
using DryIoc.WebApi.Owin;
using Microsoft.Owin;
using Owin;
using mobSocial.WebApi;
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

            //route registrations & other configurations
            WebApiConfig.Register(config);

#if DEBUG
            app.UseErrorPage(new ErrorPageOptions());
#endif
            //run owin startup configurations from plugins
            OwinStartupManager.RunAllOwinConfigurations(app);

            

            //webapi
            app.UseWebApi(config);

            //di
            app.UseDryIocWebApi(config);

        }
    }
}