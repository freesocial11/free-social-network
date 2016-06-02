
using System.Threading.Tasks;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using Microsoft.Owin.Security.OAuth;

namespace mobSocial.WebApi.Configuration.Security.AuthenticationProviders
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userService = mobSocialEngine.ActiveEngine.Resolve<IUserService>();
            //get the user with the email
            var user = userService.FirstOrDefault(x => x.Email == context.UserName || x.UserName == context.UserName);
            if (user == null)
            {
                context.SetError("invalid_grant", "The username or password provided is incorrect");
                return null;
            }
            var cryptographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();

            //get hashed password
            var hashedPassword = cryptographyService.GetHashedPassword(context.Password, user.PasswordSalt, user.PasswordFormat);
            if (user.Password != hashedPassword)
            {
                context.SetError("invalid_grant", "The username or password provided is incorrect");
                return null;
            }

        }
    }
}