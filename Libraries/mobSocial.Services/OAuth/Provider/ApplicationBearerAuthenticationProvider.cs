using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Services.Security;
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
                var crypotographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
                var tokenHash = crypotographyService.Encrypt(context.Token);
                var tokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();
                //get the token and check if it has been revoked?
                var token = tokenService.FirstOrDefault(x => x.ProtectedTicket == tokenHash);
                if (token == null || token.ExpiresUtc < DateTime.UtcNow)
                {
                    if(token != null)
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

            OAuthApplication client = null;
            if (!reject)
            {
                client = applicationService.FirstOrDefault(x => x.Guid == clientid && x.Active);
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
            //set active client to use down the line
            context.OwinContext.Set(OwinEnvironmentVariableNames.ActiveClient, client);
            return base.ValidateIdentity(context);
        }
    }
}