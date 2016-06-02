using System;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Plugins.OAuth.Entity.OAuth;
using mobSocial.Plugins.OAuth.Enums;
using mobSocial.Plugins.OAuth.Services;
using Microsoft.Owin.Security.Infrastructure;

namespace mobSocial.Plugins.OAuth.Provider
{
    public class ApplicationAuthorizationCodeProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {

            var clientid = context.OwinContext.Get<string>("as:client_id");

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var appTokenId = Guid.NewGuid().ToString("n");

            var appTokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();

            var tokenLifeTime = context.OwinContext.Get<string>("as:clientAccessTokenLifeTime");

            var token = new AppToken() {
                Guid = Helper.GetHash(appTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenLifeTime)),
                TokenType = TokenType.AccessToken
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            appTokenService.Insert(token);

            context.SetToken(appTokenId);
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
            return Task.FromResult(0);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            var hashedTokenId = Helper.GetHash(context.Token);
            var appTokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();

            var appToken = appTokenService.FirstOrDefault(x => x.Guid == hashedTokenId);
            if (appToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(appToken.ProtectedTicket);
                appTokenService.Delete(appToken);
            }

        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
            return Task.FromResult(0);
        }
    }
}