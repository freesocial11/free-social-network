using System;
using System.Collections.Concurrent;
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


            var tokenLifeTime = context.OwinContext.Get<string>("as:clientAccessTokenLifeTime");

            var token = new AppToken() {
                Guid = OAuthHelper.GetHash(appTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenLifeTime)),
                TokenType = TokenType.AccessToken
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            _tokens.TryAdd(token.Guid, token);

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

            _tokens.TryGetValue(hashedTokenId, out AppToken appToken);
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