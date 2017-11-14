using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Data.Enum;
using mobSocial.Services.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace mobSocial.Services.OAuth.Provider
{
    public class ApplicationAccessTokenProvider : IAuthenticationTokenProvider
    {

        public void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.Dictionary.TryGetValue("client_id", out string clientid);
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            //for implicit grants, we won't be saving the access tokens as the request has alrady ended //https://tools.ietf.org/html/rfc6749
            var isImplicitGrant = context.Request.Query.Get("response_type") == "token"; //according to oauth spec response_type is token for implicit grant
            var tokenLifeTime = 0;
            OAuthApplication client = null;
            if (!isImplicitGrant)
            {
                var applicationService = mobSocialEngine.ActiveEngine.Resolve<IApplicationService>();
                client = applicationService.FirstOrDefault(x => x.Guid == clientid && x.Active);
                if (client == null)
                    return;

            }
            tokenLifeTime = client?.ApplicationType == ApplicationType.Native
                ? (365 * 24 * 60) //a one year valid access token unless revoked
                : OAuthConstants.AccessTokenExpirationSecondsForNonNativeApplications /
                  60; // for javascript applications



            var guid = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            AppToken token = null;
            IAppTokenService tokenService = null;
            if (!isImplicitGrant)
            {
                tokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();
                token = tokenService.FirstOrDefault(x => x.Guid == guid && x.ClientId == clientid);
            }


            token = token ?? new AppToken() {
                Guid = guid,
                ClientId = clientid,
                Subject = email ?? guid,
                TokenType = TokenType.AccessToken
            };
            token.IssuedUtc = DateTime.UtcNow;
            token.ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenLifeTime));

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            var accessToken = context.SerializeTicket();

            if (!isImplicitGrant)
            {
                var crypotographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
                token.ProtectedTicket = crypotographyService.Encrypt(accessToken);
                if (token.Id > 0)
                    tokenService.Update(token);
                else
                {
                    tokenService.Insert(token);
                }
            }
            context.SetToken(accessToken);
        }


        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
            return Task.FromResult(0);
        }


        public void Receive(AuthenticationTokenReceiveContext context)
        {

        }


        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
            return Task.FromResult(0);
        }
    }
}