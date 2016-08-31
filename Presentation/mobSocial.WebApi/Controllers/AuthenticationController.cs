using System;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Security;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Authentication;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("authentication")]
    public class AuthenticationController : RootApiController
    {
        #region variables

        private readonly IUserService _userService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IMediaService _mediaService;
        private readonly IFollowService _followService;
        private readonly IFriendService _friendService;
        private readonly IRoleService _roleService;
        private readonly MediaSettings _mediaSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly UserSettings _userSettings;
        #endregion

        #region ctor

        public AuthenticationController(IUserService userService,
            ICryptographyService cryptographyService, IMediaService mediaService, MediaSettings mediaSettings, IFollowService followService, IFriendService friendService, IUserRegistrationService userRegistrationService, SecuritySettings securitySettings, UserSettings userSettings, IRoleService roleService)
        {
            _userService = userService;
            _cryptographyService = cryptographyService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _followService = followService;
            _friendService = friendService;
            _userRegistrationService = userRegistrationService;
            _securitySettings = securitySettings;
            _userSettings = userSettings;
            _roleService = roleService;
        }

        #endregion


        #region actions

        [HttpPost]
        [Route("login", Name = "DefaultAuthenticationLogin")]
        public IHttpActionResult Login(LoginModel loginModel)
        {
            var redirect = false;

            if (loginModel == null || !ModelState.IsValid || !ShouldSignIn(loginModel.Email, loginModel.Password))
            {
                VerboseReporter.ReportError("The email or password is invalid", "login");
                return RespondFailure();
            }

            //sign in the current user
            var loginStatus = ApplicationContext.Current.SignIn(loginModel.Email, loginModel.Persist);
            if (loginStatus == LoginStatus.Success)
            {
                //update the last login date & ip address
                var currentUser = ApplicationContext.Current.CurrentUser;
                currentUser.LastLoginDate = DateTime.UtcNow;
                currentUser.LastLoginIpAddress = WebHelper.GetClientIpAddress();
                _userService.Update(currentUser);

                VerboseReporter.ReportSuccess("Your login was successful", "login");
                return RespondSuccess(new {
                    ReturnUrl = loginModel.ReturnUrl,
                    User = ApplicationContext.Current.CurrentUser.ToModel(_mediaService, _mediaSettings, _followService, _friendService)
                });

            }
            VerboseReporter.ReportError("The login attempt failed due to unknown error", "login");
            return RespondFailure();

        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            ApplicationContext.Current.SignOut();
            VerboseReporter.ReportSuccess("You have been successfully logged out", "logout");
            return RespondSuccess();
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(RegisterModel registerModel)
        {
            const string contextName = "register";

            if (!ModelState.IsValid)
                return RespondFailure("All the fields are required to complete the registration", contextName);

            if(string.Compare(registerModel.Password, registerModel.ConfirmPassword, StringComparison.InvariantCulture) != 0)
                return RespondFailure("The passwords do not match", contextName);

            if(!registerModel.Agreement)
                return RespondFailure("You must agree to the terms & conditions to complete the registration", contextName);

            //we can now try to register this user
            //so create a new object
            var user = new User()
            {
                Email = registerModel.Email,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Name = $"{registerModel.FirstName} {registerModel.LastName}",
                Password = registerModel.Password,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                IsSystemAccount = false,
                ReferrerId = registerModel.ReferrerId,
                Guid = Guid.NewGuid(),
                Active = _userSettings.UserRegistrationDefaultMode == RegistrationMode.Immediate
            };
            //register this user
            var registrationStatus = _userRegistrationService.Register(user, _securitySettings.DefaultPasswordStorageFormat);
            if(registrationStatus == UserRegistrationStatus.FailedAsEmailAlreadyExists)
                return RespondFailure("A user with this email is already registered", contextName);

            //assign role to the user
            _roleService.AssignRoleToUser(SystemRoleNames.Registered, user);
            return RespondSuccess();

        }

        #endregion

        #region utilities

        [NonAction]
        private bool ShouldSignIn(string email, string password)
        {
            //get the user with the email
            var user = _userService.FirstOrDefault(x => x.Email == email);
            if (user == null)
                return false;

            //get hashed password
            var hashedPassword = _cryptographyService.GetHashedPassword(password, user.PasswordSalt, user.PasswordFormat);
            return user.Password == hashedPassword;
        }
        #endregion
    }
}