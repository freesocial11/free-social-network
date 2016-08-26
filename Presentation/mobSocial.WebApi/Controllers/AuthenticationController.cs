using System;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Data.Entity.Settings;
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
        private readonly ICryptographyService _cryptographyService;
        private readonly IMediaService _mediaService;
        private readonly IFollowService _followService;
        private readonly IFriendService _friendService;
        private readonly MediaSettings _mediaSettings;
        #endregion

        #region ctor

        public AuthenticationController(IUserService userService,
            ICryptographyService cryptographyService, IMediaService mediaService, MediaSettings mediaSettings, IFollowService followService, IFriendService friendService)
        {
            _userService = userService;
            _cryptographyService = cryptographyService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _followService = followService;
            _friendService = friendService;
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