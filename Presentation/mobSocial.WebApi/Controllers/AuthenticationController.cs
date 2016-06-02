using System.Web.Http;
using mobSocial.Data.Enum;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.Authentication;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("authentication")]
    public class AuthenticationController : RootApiController
    {
        #region variables

        private readonly IUserService _userService;
        private readonly ICryptographyService _cryptographyService;
        #endregion

        #region ctor

        public AuthenticationController(IUserService userService,
            ICryptographyService cryptographyService)
        {
            _userService = userService;
            _cryptographyService = cryptographyService;
        }

        #endregion


        #region actions

        [HttpPost]
        [Route("login", Name = "DefaultAuthenticationLogin")]
        public IHttpActionResult Login(LoginModel loginModel)
        {
            var redirect = false;

            if (loginModel == null || !ModelState.IsValid)
            {
                if (!ModelState.IsValid && loginModel != null)
                    redirect = loginModel.Redirect;

                if (!redirect)
                {
                    VerboseReporter.ReportError("The email or password is invalid", "login");
                    return RespondFailure();
                }
                return RespondRedirect(Request.RequestUri);

            }

            redirect = loginModel.Redirect;
            if (!ShouldSignIn(loginModel.Email, loginModel.Password))
            {
                if (!redirect)
                {
                    VerboseReporter.ReportError("The email or password is invalid", "login");
                    return RespondFailure();
                }
                return RespondRedirect(Request.RequestUri);
            }

            //sign in the current user
            var loginStatus = ApplicationContext.Current.SignIn(loginModel.Email, loginModel.Persist);
            if (loginStatus == LoginStatus.Success)
            {
                if (!redirect)
                {
                    VerboseReporter.ReportSuccess("Your login was successful", "login");
                    return RespondSuccess(new {
                        ReturnUrl = loginModel.ReturnUrl
                    });
                }
                return RespondRedirect(loginModel.ReturnUrl);

            }
            if (!redirect)
            {
                VerboseReporter.ReportError("The login attempt failed due to unknown error", "login");
                return RespondFailure();
            }
            return RespondRedirect(Request.RequestUri);

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