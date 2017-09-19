using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Data.Enum;
using mobSocial.Services.Helpers;
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
            var applicationService = mobSocialEngine.ActiveEngine.Resolve<IApplicationService>();
            var client = applicationService.FirstOrDefault(x => x.Guid == clientid && x.Active);
            if (client == null)
                return;

            var tokenLifeTime = client.ApplicationType == ApplicationType.Native
                ? (365 * 24 * 60) //a one year valid access token unless revoked
                : OAuthConstants.AccessTokenExpirationSecondsForNonNativeApplications / 60;// for javascript applications

            var guid = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            var tokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();
            var token = tokenService.FirstOrDefault(x => x.Guid == guid && x.ClientId == clientid);

            token = token ?? new AppToken()
            {
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
            token.ProtectedTicket = OAuthHelper.GetHash(accessToken);
            if (token.Id > 0)
                tokenService.Update(token);
            else
            {
                tokenService.Insert(token);
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