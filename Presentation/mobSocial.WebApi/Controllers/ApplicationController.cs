using System;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.OAuth;
using mobSocial.Services.Extensions;
using mobSocial.Services.OAuth;
using mobSocial.Services.Security;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Applications;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("apps")]
    [Authorize]
    public class ApplicationController : RootApiController
    {
        private readonly IApplicationService _applicationService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IAppTokenService _appTokenService;

        public ApplicationController(IApplicationService applicationService, ICryptographyService cryptographyService, IAppTokenService appTokenService)
        {
            _applicationService = applicationService;
            _cryptographyService = cryptographyService;
            _appTokenService = appTokenService;
        }

        [Route("get/all")]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var applications = _applicationService.Get(x => x.UserId == currentUser.Id).ToList();
            var model = applications.Select(x => x.ToMiniModel());
            return RespondSuccess(new
            {
                Applications = model
            });
        }

        [Route("get/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var application = _applicationService.FirstOrDefault(x => x.UserId == currentUser.Id && x.Id == id);
            if (application == null)
                return NotFound();

            var model = application.ToModel();
            return RespondSuccess(new {
                Application = model
            });
        }

        [HttpPost]
        [Route("post")]
        public IHttpActionResult Post(ApplicationPostModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            OAuthApplication application = null;
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (requestModel.Id != 0)
            {
                //get the existing app
                application = _applicationService.Get(requestModel.Id);
                if (application == null || !CanCurrentUserEdit(application))
                    return NotFound();
            }
            else
            {
                application = new OAuthApplication
                {
                    //generate random clientid and clientsecret
                    Guid = Guid.NewGuid().ToString("N"),
                    Secret = _cryptographyService.CreateSalt(OAuthConstants.ClientSecretLength)
                };

            }

            //update values
            application.Name = requestModel.Name;
            application.Active = requestModel.Active;
            application.RedirectUrl = requestModel.RedirectUrl;
            application.PrivacyPolicyUrl = requestModel.PrivacyPolicyUrl;
            application.TermsUrl = requestModel.TermsUrl;
            application.Description = requestModel.Description;
            application.UserId = currentUser.Id;
            application.ApplicationType = requestModel.ApplicationType;
            application.AllowedOrigin = requestModel.AllowedOrigin;
           
            if(application.Id == 0)
                _applicationService.Insert(application);
            else
                _applicationService.Update(application);

            var model = application.ToModel();
            return RespondSuccess(new {
                Application = model
            });

        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var application = _applicationService.Get(id);
            if (application == null || !CanCurrentUserEdit(application))
                return NotFound();

            //delete the application and it's tokens
            _appTokenService.Delete(x => x.ClientId == application.Guid);
            _applicationService.Delete(application);
            return RespondSuccess();
        }

        [HttpPut]
        [Route("put/secret/{id:int}")]
        public IHttpActionResult UpdateSecret(int id)
        {
            var application = _applicationService.Get(id);
            if (application == null || !CanCurrentUserEdit(application))
                return NotFound();
            //delete all the existing tokens for the client app
            _appTokenService.Delete(x => x.ClientId == application.Guid);
            //update the application secret
            application.Secret = _cryptographyService.CreateSalt(OAuthConstants.ClientSecretLength);
            _applicationService.Update(application);

            var model = application.ToModel();
            return RespondSuccess(new {
                Application = model
            });
        }

        [NonAction]
        public bool CanCurrentUserEdit(OAuthApplication application)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            return application.UserId == currentUser.Id || currentUser.IsAdministrator();
        }
    }
}