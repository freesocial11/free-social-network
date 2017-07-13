using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
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
            var enableCors = WebConfigurationManager.AppSettings["enableCors"] != null &&
                             WebConfigurationManager.AppSettings["enableCors"].ToLower() == "true";

            if (enableCors)
            {
                var origins = WebConfigurationManager.AppSettings["corsAllowedOrigins"] ?? "*";
                var cors = new EnableCorsAttribute(origins, "*", "GET,POST,PUT,DELETE");
                config.EnableCors(cors);
            };
           
            //route registrations & other configurations
            WebApiConfig.Register(config);

            app.MapWhen(x => x.Request.Uri.AbsolutePath.StartsWith("/" + WebApiConfig.ApiPrefix), builder =>
            {
                builder.UseInstallationVerifier();

                //run owin startup configurations from plugins
                OwinStartupManager.RunAllOwinConfigurations(app);

                //builder.UseDryIocOwinMiddleware(mobSocialEngine.ActiveEngine.IocContainer);

                builder.UsePictureSizeRegistrar();

#if DEBUG
                builder.UseErrorPage(new ErrorPageOptions());
#endif


                //webapi, last one always 
                builder.UseWebApi(config);
            });

            app.MapWhen(x => x.Request.Uri.AbsolutePath.StartsWith("/signalr"), builder =>
            {
                var userIdProvider = new SignalRUserIdProvider();
                GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);
                builder.MapSignalR();
            });
        }
    }
}