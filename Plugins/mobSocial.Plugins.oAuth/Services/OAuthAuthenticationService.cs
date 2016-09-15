using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Authentication;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using Microsoft.Owin;
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
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString())
            }, "Application"));
        }

        public override void ClearAuthenticationTicket()
        {
            ApplicationContext.Current.CurrentOwinContext.Authentication.SignOut("Application");
            // clear authentication cookie
            // delete cookie from applicationcookiemanager is not working somehow. let's delete the cookie manually
            var cookie = new HttpCookie(".AspNet.Application", "") { Expires = DateTime.Now.AddYears(-1), HttpOnly = true, Path = "/"};
            ApplicationContext.Current.CurrentOwinContext.Response.Cookies.Append(".AspNet.Application", "", new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(-1),
                HttpOnly = true,                
                Path = "/"
            });
            ApplicationContext.Current.CurrentHttpContext.Response.Cookies.Add(cookie);
        }

        public override User GetCurrentUser()
        {
            if (_user != null)
                return _user;

            var ctx = ApplicationContext.Current.CurrentOwinContext;
            var user = ctx.Authentication.User;
            if (user?.Claims == null)
                return null;
            var claims = user.Claims.ToList();

            //find claim which stores the email
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            //also verify if user's GUID hasn't changed
            var uidClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (emailClaim == null || uidClaim == null)
                return base.GetCurrentUser();
           

            _user = _userService.FirstOrDefault(x => x.Email == emailClaim.Value, x => x.UserRoles.Select(y => y.Role)) ?? base.GetCurrentUser();
            return _user.Guid.ToString() == uidClaim.Value ? _user : null;
        }
    }
}