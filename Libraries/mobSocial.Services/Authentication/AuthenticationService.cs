using System;
using System.Web;
using System.Web.Security;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
using mobSocial.Services.Security;
using mobSocial.Services.Users;

namespace mobSocial.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly SecuritySettings _securitySettings;
        private readonly ICryptographyService _cryptographyService;
        private readonly HttpContextBase _contextBase;
        private User _user;

        public AuthenticationService(IUserService userService, 
            IUserRegistrationService userRegistrationService, 
            SecuritySettings securitySettings, 
            ICryptographyService cryptographyService, HttpContextBase contextBase)
        {
            _userService = userService;
            _userRegistrationService = userRegistrationService;
            _securitySettings = securitySettings;
            _cryptographyService = cryptographyService;
            _contextBase = contextBase;
        }

        public virtual LoginStatus SignIn(string email, bool isPersistent = false, bool forceCreateNewAccount = false)
        {
            var user = _userService.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                //do we need to force create a new user
                if (forceCreateNewAccount)
                {
                    var password = _cryptographyService.GetRandomPassword();
                    //register now
                    var registrationStatus = _userRegistrationService.Register(email, password, _securitySettings.DefaultPasswordStorageFormat);

                    if(registrationStatus == UserRegistrationStatus.FailedAsEmailAlreadyExists)
                        return LoginStatus.Failed;

                    //load user again
                    if (registrationStatus == UserRegistrationStatus.Success)
                        user = _userService.FirstOrDefault(x => x.Email == email);
                }
            }
            if (user == null)
                return LoginStatus.FailedUserNotExists;

            if(user.Deleted)
                return LoginStatus.FailedDeletedUser;

            if(user.IsSystemAccount)
                return LoginStatus.Failed;

            if(!user.Active)
                return LoginStatus.FailedInactiveUser;

            //create the authentication ticket for the user
            CreateAuthenticationTicket(user, isPersistent);

            return LoginStatus.Success;
        }

        public virtual void CreateAuthenticationTicket(User user, bool isPersistent = false)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            //create a new form authentication ticket
            var ticket = new FormsAuthenticationTicket(1, user.Email, now, now.Add(FormsAuthentication.Timeout),
                isPersistent,
                user.Id.ToString(), 
                FormsAuthentication.FormsCookiePath);


            //encrypt the ticket
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {HttpOnly = true};
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            _contextBase.Response.Cookies.Add(cookie);
            //set this user as current user
            _user = user;
        }

        public virtual void ClearAuthenticationTicket()
        {
            _user = null;
            FormsAuthentication.SignOut();
        }

        public virtual User GetCurrentUser()
        {
            if (_user != null)
                return _user;

            var httpContextBase = _contextBase;
            if (httpContextBase == null || 
                !httpContextBase.Request.IsAuthenticated)
                return null;

            var user = httpContextBase.User.Identity as FormsIdentity;
            //is it correct?
            if (user == null)
                return null;

            //get the authentication ticket
            var ticket = user.Ticket;
            if (ticket == null)
                return null;

            int userId;
            if (!int.TryParse(ticket.UserData, out userId))
                return null;

            var userEntity = _userService.Get(userId);
            if (userEntity == null || !userEntity.Active || userEntity.Deleted || userEntity.IsSystemAccount || !userEntity.IsRegistered())
                return null;

            //store it for future references
            _user = userEntity;
            return userEntity;
        }
    }
}