using System.Linq;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Services.Security;

namespace mobSocial.WebApi.Helpers
{
    public static class OAuthHelpers
    {
        public static string[] GetCurrentScopes(AppToken appToken)
        {
            var cryptographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
            var savedAccessToken = cryptographyService.Decrypt(appToken.ProtectedTicket);
            var dataFormat = Services.OAuth.App_Start.OwinStartup.BearerOptions.AccessTokenFormat;
            var savedTicket = dataFormat.Unprotect(savedAccessToken);
            var savedScopes = savedTicket.Identity.Claims.Where(x => x.Type == "urn:oauth:scope")
                .Select(x => x.Value).ToArray();
            return savedScopes;
        }
    }
}