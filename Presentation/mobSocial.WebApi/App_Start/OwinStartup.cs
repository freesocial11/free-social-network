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
                builder.UseDryIocOwinMiddleware(mobSocialEngine.ActiveEngine.IocContainer);

                var userIdProvider = new SignalRUserIdProvider();
                GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);
                builder.MapSignalR();
            });
        }
    }
}