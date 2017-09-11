using System;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Data.Enum;
using mobSocial.Services.Helpers;
using Microsoft.Owin.Security.Infrastructure;

namespace mobSocial.Services.OAuth.Provider
{
    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.OwinContext.Get<string>("as:client_id");

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new AppToken() {
                Guid = OAuthHelper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                TokenType = TokenType.RefreshToken
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            refreshTokenService.Insert(token);

            context.SetToken(refreshTokenId);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {


            var hashedTokenId = OAuthHelper.GetHash(context.Token);
            var refreshTokenService = mobSocialEngine.ActiveEngine.Resolve<IAppTokenService>();

            var refreshToken = await refreshTokenService.FirstOrDefaultAsync(x => x.Guid == hashedTokenId);
            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                refreshTokenService.Delete(refreshToken);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            CreateAsync(context);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            ReceiveAsync(context);
        }
    }
}
