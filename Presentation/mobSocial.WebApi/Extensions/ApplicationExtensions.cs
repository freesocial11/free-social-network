using System.Linq;
using mobSocial.Core.Caching;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Services.OAuth;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Helpers;

namespace mobSocial.WebApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static bool HasScope(this OAuthApplication application, string scope, bool allowForNullApplications = false)
        {
            if (application == null && allowForNullApplications)
                return true;
            return application != null && application.GetScopes().Any(x => x == scope);
        }

        public static string[] GetScopes(this OAuthApplication application)
        {
            var applicationScopeKey = $"oauthapp_scopes_{application.Id}";
            var cacheProvider = mobSocialEngine.ActiveEngine.Resolve<ICacheProvider>();
            var appTokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();
            return cacheProvider.Get(applicationScopeKey, () =>
            {
                var currentToken =
                    appTokenService.FirstOrDefault(
                        x => x.Guid == ApplicationContext.Current.CurrentUser.Guid.ToString() &&
                             x.ClientId == application.Guid);
                return OAuthHelpers.GetCurrentScopes(currentToken);
            });
        }
    }
}