using System;
using System.Collections.Generic;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Extensions;
using mobSocial.Services.Emails;
using mobSocial.Services.EntityProperties;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Notifications;
using mobSocial.Services.OAuth;
using mobSocial.Services.Security;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Authentication;
using mobSocial.WebApi.Models.Users;

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
        private readonly IEmailSender _emailSender;
        private readonly UrlSettings _urlSettings;
        private readonly IEntityPropertyService _entityPropertyService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationService _applicationService;
        #endregion

        #region ctor

        public AuthenticationController(IUserService userService,
            ICryptographyService cryptographyService, IMediaService mediaService, MediaSettings mediaSettings, IFollowService followService, IFriendService friendService, IUserRegistrationService userRegistrationService, SecuritySettings securitySettings, UserSettings userSettings, IRoleService roleService, IEmailSender emailSender, UrlSettings urlSettings, IEntityPropertyService entityPropertyService, INotificationService notificationService, IApplicationService applicationService)
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
            _emailSender = emailSender;
            _urlSettings = urlSettings;
            _entityPropertyService = entityPropertyService;
            _notificationService = notificationService;
            _applicationService = applicationService;
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
                    User = ApplicationContext.Current.CurrentUser.ToModel(_mediaService, _mediaSettings, _followService, _friendService, _notificationService)
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

            if (string.Compare(registerModel.Password, registerModel.ConfirmPassword, StringComparison.InvariantCulture) != 0)
                return RespondFailure("The passwords do not match", contextName);

            if (!registerModel.Agreement)
                return RespondFailure("You must agree to the terms & conditions to complete the registration", contextName);

            //we can now try to register this user
            //so create a new object
            var user = new User() {
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
            if (registrationStatus == UserRegistrationStatus.FailedAsEmailAlreadyExists)
                return RespondFailure("A user with this email is already registered", contextName);

            //assign role to the user
            _roleService.AssignRoleToUser(SystemRoleNames.Registered, user);

            //so we are done, send a notification to user and admin
            _emailSender.SendUserRegisteredMessage(user);
            var responseMessage = "Your account has been successfully created.";
            if (_userSettings.UserRegistrationDefaultMode == RegistrationMode.WithActivationEmail)
            {
                SendActivationEmail(user);
                responseMessage += " Email verification is required before you can login. Please check your inbox for activation link.";
            }
            return RespondSuccess(responseMessage, contextName);
        }

        [HttpPost]
        [Route("register2")]
        public IHttpActionResult Register2(WithCustomFieldsModel<RegisterModel> requestModel)
        {
            const string contextName = "register";
            var registerModel = requestModel.Model;

            var application = _applicationService.FirstOrDefault(x => x.Guid == requestModel.Model.ClientId);
            if(application == null)
                return RespondFailure("Unknown client application", contextName);

            if (!requestModel.ValidateCustomFieldsForEntity<User>(application.Id))
                return RespondFailure("Some of the required fields are missing to complete the registration", contextName);

            if (string.Compare(registerModel.Password, registerModel.ConfirmPassword, StringComparison.InvariantCulture) != 0)
                return RespondFailure("The passwords do not match", contextName);

            if (!registerModel.Agreement)
                return RespondFailure("You must agree to the terms & conditions to complete the registration", contextName);

            //we can now try to register this user
            //so create a new object
            var user = new User() {
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
            if (registrationStatus == UserRegistrationStatus.FailedAsEmailAlreadyExists)
                return RespondFailure("A user with this email is already registered", contextName);

            //assign role to the user
            _roleService.AssignRoleToUser(SystemRoleNames.Registered, user);

            //save the custom fields as entity properties
            foreach (var cs in requestModel.SubmittedCustomFields)
            {
                user.SetPropertyValue(cs.FieldName, cs.FieldValue);
            }

            //save the application id as well
            user.SetPropertyValue("ClientId", requestModel.Model.ClientId);
            //so we are done, send a notification to user and admin
            _emailSender.SendUserRegisteredMessage(user);
            var responseMessage = "Your account has been successfully created.";
            if (_userSettings.UserRegistrationDefaultMode == RegistrationMode.WithActivationEmail)
            {
                SendActivationEmail(user);
                responseMessage += " Email verification is required before you can login. Please check your inbox for activation link.";
            }
            return RespondSuccess(responseMessage, contextName);
        }

        [HttpPost]
        [Route("activate")]
        public IHttpActionResult Activate(UserActivationModel activationModel)
        {
            const string contextName = "activate";
            if(string.IsNullOrEmpty(activationModel.ActivationCode))
                return RespondFailure("Invalid activation code", contextName);

            if(_userSettings.RequireEmailForUserActivation && string.IsNullOrEmpty(activationModel.Email))
                return RespondFailure("Invalid email", contextName);

            //let's see if we have correct activation code
            if (_userSettings.RequireEmailForUserActivation)
            {
                //let's find the user who has this email
                var user = _userService.FirstOrDefault(x => x.Email == activationModel.Email);
                if (user == null)
                    return RespondFailure("The email is not registered", contextName);

                if (user.Active)
                    return RespondFailure("The user is already active", contextName);

                //get the activation code and verify if it's same
                var savedActivationCode = user.GetPropertyValueAs<string>(PropertyNames.ActivationCode);

                if (string.Compare(activationModel.ActivationCode, savedActivationCode, StringComparison.Ordinal) != 0)
                    return RespondFailure("Invalid activation code", contextName);

                //activate the user now
                user.Active = true;
                _userService.Update(user);

                //delete the activation code as well
                user.DeleteProperty(PropertyNames.ActivationCode);

                //send notification
                _emailSender.SendUserActivatedMessage(user);
                return RespondSuccess("Your email has been successfully verified", contextName);
            }
            else
            {
                //we just need to find a user whose activation code matches with provided code and mark it active
                var property = _entityPropertyService.FirstOrDefault(
                    x =>
                        x.PropertyName == PropertyNames.ActivationCode && x.Value == activationModel.ActivationCode &&
                        x.EntityName == typeof(User).Name);

                if(property == null)
                    return RespondFailure("Invalid activation code", contextName);

                //get the user with this property
                var user = _userService.Get(property.EntityId);
                if(user == null)
                    return RespondFailure("The user account doesn't exist or has been deleted", contextName);

                if (user.Active)
                    return RespondFailure("The user is already active", contextName);

                //mark the user as active
                user.Active = true;
                _userService.Update(user);

                //delete the activation code as well
                user.DeleteProperty(PropertyNames.ActivationCode);

                //send notification
                _emailSender.SendUserActivatedMessage(user);

                return RespondSuccess("Your email has been successfully verified", contextName);

            }
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

        [NonAction]
        private void SendActivationEmail(User user)
        {
            //we need to send activation email, so let's first generate the activation code.
            //we use random password generator to get a unique activation code
            var activationCode = _cryptographyService.GetHashedPassword(Guid.NewGuid().ToString(), user.PasswordSalt,
                PasswordFormat.Sha256Hashed);
            //save the property value
            user.SetPropertyValue(PropertyNames.ActivationCode, activationCode);

            //and send the activation url with email
            var activationUrl = WebHelper.ParseUrl(_urlSettings.ActivationPageUrl, new Dictionary<string, string>()
            {
                {"code", activationCode}
            });

            //send the email now
            _emailSender.SendUserActivationLinkMessage(user, activationUrl.AbsoluteUri);

        }

        #endregion
    }
}