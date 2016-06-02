using System.Linq;
using System.Security.Claims;
using System.Web;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Authentication;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using Microsoft.Owin.Security;

namespace mobSocial.Plugins.OAuth.Services
{
    public class OAuthAuthenticationService : AuthenticationService
    {
        private readonly IUserService _userService;

        private User _user;

        public OAuthAuthenticationService(IUserService userService,
            IUserRegistrationService userRegistrationService,
            SecuritySettings securitySettings,
            ICryptographyService cryptographyService,
            HttpContextBase contextBase) : base(userService, userRegistrationService, securitySettings, cryptographyService, contextBase)
        {
            _userService = userService;
        }

        public override void CreateAuthenticationTicket(User user, bool isPersistent = false)
        {
            //get the current owin authentication
            var ctx = ApplicationContext.Current.CurrentOwinContext;
            var authentication = ctx.Authentication;

            //sign in the user. this will create the authentication cookie
            authentication.SignIn(new AuthenticationProperties()
            {
                IsPersistent = isPersistent
            }, new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            }, "Application"));
        }

        public override User GetCurrentUser()
        {
            if (_user != null)
                return _user;

            var ctx = ApplicationContext.Current.CurrentOwinContext;
            var user = ctx.Authentication.User;
            var claims = user.Claims;

            //find claim which stores the email
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (emailClaim == null)
                return base.GetCurrentUser();

            _user = _userService.FirstOrDefault(x => x.Email == emailClaim.Value) ?? base.GetCurrentUser();
            return _user;
        }
    }
}