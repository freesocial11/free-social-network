using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Helpers;
using mobSocial.Services.Users;
using Microsoft.Owin.Security.OAuth;

namespace mobSocial.Services.OAuth.Provider
{
    public class ApplicationBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (!string.IsNullOrEmpty(context.Token))
            {
                var tokenHash = OAuthHelper.GetHash(context.Token);
                var tokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();
                //get the token and check if it has been revoked?
                var token = tokenService.FirstOrDefault(x => x.ProtectedTicket == tokenHash);
                if (token == null || token.ExpiresUtc < DateTime.UtcNow)
                {
                    tokenService.Delete(token);
                    context.Token = null;
                }
                else
                {
                    //reissue the token //todo: analyze if it's a good way to achieve a self refreshing access token
                    token.IssuedUtc = DateTime.UtcNow;
                    token.ExpiresUtc =
                        token.IssuedUtc.AddMinutes(Convert.ToDouble(OAuthConstants.AccessTokenExpirationSecondsForNativeApplications / 60));
                    tokenService.Update(token);
                }
            }
            return base.RequestToken(context);
        }

        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            //client validation
            var applicationService = mobSocialEngine.ActiveEngine.Resolve<IApplicationService>();
            context.Ticket.Properties.Dictionary.TryGetValue("client_id", out string clientid);
            var reject = string.IsNullOrEmpty(clientid);

            if (!reject)
            {
                var client = applicationService.FirstOrDefault(x => x.Guid == clientid && x.Active);
                reject = client == null;
            }

            if (!reject)
            {
                //check if the user exist?
                var userService = mobSocialEngine.ActiveEngine.Resolve<IUserService>();
                var guid = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (guid == null)
                {
                    reject = true;
                }
                else
                {
                    var guidObject = Guid.Parse(guid);
                    var user = userService.FirstOrDefault(x => x.Guid == guidObject);
                    if (user == null || !user.Active)
                    {
                        reject = true;
                    }
                }
            }

            if (reject)
            {
                context.Rejected();
                return Task.FromResult(0);
            }
         
            return base.ValidateIdentity(context);
        }
    }
}