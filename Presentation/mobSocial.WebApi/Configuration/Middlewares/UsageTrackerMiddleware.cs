using System.Net;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.OAuth;
using mobSocial.WebApi.Configuration.Infrastructure;
using Owin;

namespace mobSocial.WebApi.Configuration.Middlewares
{
    public static class UsageTrackerMiddleware
    {
        public static void TrackApiUsage(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var appUsageService = mobSocialEngine.ActiveEngine.Resolve<IApplicationUsageService>();
                var application = ApplicationContext.Current.CurrentOAuthApplication;
                if (application != null)
                {
                    if (appUsageService.IsUsageAllowed(application.Id))
                        appUsageService.TrackUsage(application.Id);
                    else
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync("{\"message\" : \"The application has execeeded allowed call limits\"}");
                        return;
                    }
                }
                await next();
            });
        }
    }
};