using System.Web;
using mobSocial.Data.Database;
using mobSocial.WebApi.Configuration.Infrastructure;
using Owin;

namespace mobSocial.WebApi.Configuration.Middlewares
{
    /// <summary>
    /// We'll use this to perform various login checks such as IpAddress verification, login status, bruteforce attacks etc.
    /// </summary>
    public static class MobAuthenticationMiddleware
    {
        public static void UseMobAuthentication(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (DatabaseManager.IsDatabaseInstalled() && ApplicationContext.Current.CurrentUser == null)
                {
                    //avoid login page from this check, otherwise, we won't be able to login ever
                    if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/login") ||
                        HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/register"))
                    {
                        await next();
                        return;

                    }
                    HttpContext.Current.User = null;
                    HttpContext.Current.GetOwinContext().Authentication.User = null;
                }
                await next();
            });
        }
    }
}