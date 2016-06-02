using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Plugins.OAuth.Entity.OAuth;
using mobSocial.Plugins.OAuth.Enums;
using mobSocial.Plugins.OAuth.Services;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace mobSocial.Plugins.OAuth.Provider
{
    public class ApplicationOAuthServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            OAuthClient client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            var clientService = mobSocialEngine.ActiveEngine.Resolve<IClientService>();
            client = clientService.FirstOrDefault(x => x.Guid == clientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", $"Client '{context.ClientId}' is not registered in the system.");
                return Task.FromResult<object>(null);
            }
            //native applications should also pass client secret
            if (client.ApplicationType == ApplicationType.NativeConfidential || client.ApplicationType == ApplicationType.NativeFullControl)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != Helper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.FirstOrDefault(c => c.Type == "newClaim");
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "http://mobsocial.com" });

            var userService = mobSocialEngine.ActiveEngine.Resolve<IUserService>();
            var user =
                await userService.FirstOrDefaultAsync(x => x.Email == context.UserName || x.UserName == context.UserName);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }


            var cryptographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
            var hashedPassword = cryptographyService.GetHashedPassword(context.Password, user.PasswordSalt,
                user.PasswordFormat);

            if (hashedPassword != user.Password)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, string.Join(",", user.UserRoles.Select(x => x.Role.SystemName))));

            context.OwinContext.Set<string>("as:client_id", context.ClientId);

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "userName", context.UserName
                }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            var clientService = mobSocialEngine.ActiveEngine.Resolve<IClientService>();

            var client = clientService.FirstOrDefault(x => x.Guid == context.ClientId);
            if (client != null)
                context.Validated(client.RedirectUri);

            return Task.FromResult<object>(null);
        }

        public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            var clientService = mobSocialEngine.ActiveEngine.Resolve<IClientService>();
            var client = clientService.FirstOrDefault(x => x.Guid == context.AuthorizeRequest.ClientId);

            context.OwinContext.Set<string>("as:client_id", client.Guid);
            context.OwinContext.Set<string>("as:clientAccessTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            return base.AuthorizeEndpoint(context);
        }
    }
}
