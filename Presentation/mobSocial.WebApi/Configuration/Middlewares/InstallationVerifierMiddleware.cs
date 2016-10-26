using System.Net;
using System.Web;
using mobSocial.Data.Database;
using Owin;

namespace mobSocial.WebApi.Configuration.Middlewares
{
    public static class InstallationVerifierMiddleware
    {
        public static void UseInstallationVerifier(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!DatabaseManager.IsDatabaseInstalled())
                {
                    //avoid install page from this check, otherwise, we won't be able to install ever
                    if (!HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/install"))
                    {

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        await context.Response.WriteAsync("{dbnotinstalled : true}");
                        return;
                    }
                }
                await next();
            });
        }
    }
}