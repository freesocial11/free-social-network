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
                    var absPath = HttpContext.Current.Request.Url.AbsolutePath;
                    if (absPath.StartsWith("/" + WebApiConfig.ApiPrefix))
                    {
                        //avoid install page from this check, otherwise, we won't be able to install ever
                        if (!absPath.EndsWith("/install") && !absPath.EndsWith("/install/test-connection"))
                        {

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            await context.Response.WriteAsync("{\"dbnotinstalled\" : true}");
                            return;
                        }
                    }
                  
                }
                await next();
            });
        }
    }
}