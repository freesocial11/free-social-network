using System;
using System.Collections.Concurrent;
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
    public class ApplicationAuthorizationCodeProvider : IAuthenticationTokenProvider
    {
        private readonly ConcurrentDictionary<string, AppToken> _tokens = new ConcurrentDictionary<string, AppToken>();
        public void Create(AuthenticationTokenCreateContext context)
        {

            var clientid = context.OwinContext.Get<string>("as:client_id");

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var appTokenId = Guid.NewGuid().ToString("n");


            var tokenLifeTime = OAuthConstants.AuthorizationCodeExpirationSeconds / 60;
            var guid = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = context.Ticket.Identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }
            var token = new AppToken() {
                Guid = guid,
                ClientId = clientid,
                Subject = email ?? guid,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenLifeTime)),
                TokenType = TokenType.AccessToken
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            _tokens.TryAdd(OAuthHelper.GetHash(appTokenId), token);

            context.SetToken(appTokenId);
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
            return Task.FromResult(0);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            var hashedTokenId = OAuthHelper.GetHash(context.Token);

            AppToken appToken;
            _tokens.TryGetValue(hashedTokenId, out appToken);
            if (appToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(appToken.ProtectedTicket);
                _tokens.TryRemove(hashedTokenId, out appToken);
            }

        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
            return Task.FromResult(0);
        }
    }
}