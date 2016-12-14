using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using Owin;

namespace mobSocial.WebApi.Configuration.Middlewares
{
    /// <summary>
    /// Middleware for registering various picture sizes
    /// </summary>
    public static class PictureSizeRegistrar
    {
        public static void UsePictureSizeRegistrar(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (DatabaseManager.IsDatabaseInstalled())
                {
                    using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
                    {
                        mobSocialEngine.SetupPictureSizes();
                        await next();
                        return;
                    }
                }
                await next();
            });
        }
    }
}