using System.Dynamic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Emails;
using mobSocial.Services.Emails;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Emails;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("emailtemplates")]
    [AdminAuthorize]
    public class EmailTemplateController : RootApiController
    {
        #region variables
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        public EmailTemplateController(IEmailTemplateService emailTemplateService, IEmailAccountService emailAccountService)
        {
            _emailTemplateService = emailTemplateService;
            _emailAccountService = emailAccountService;
        }

        #endregion

        [Route("get")]
        [HttpGet]
        public IHttpActionResult Get([FromUri] EmailTemplateGetModel requestModel)
        {
            if (requestModel.Count <= 0)
                requestModel.Count = 15;

            if (requestModel.Page <= 0)
                requestModel.Page = 1;

            //get the email templates
            var emailTemplates = _emailTemplateService.Get(x => !requestModel.MasterOnly || x.IsMaster, page: requestModel.Page, count: requestModel.Count).ToList();
            var model = emailTemplates.Select(x => x.ToEntityModel());
            return RespondSuccess(new {
                EmailTemplates = model
            });
        }

        [Route("get/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            //get the account
            var emailTemplate = _emailTemplateService.Get(id);
            if (emailTemplate == null)
                return NotFound();
            var model = emailTemplate.ToEntityModel();
            model.AvailableEmailAccounts = _emailAccountService.Get().ToList().Select(x =>
            {
                dynamic newObject = new ExpandoObject();
                newObject.Name = $"{x.FromName} ({x.Email})";
                newObject.Id = x.Id;
                return newObject;
            }).ToList();

            model.AvailableMasterTemplates = _emailTemplateService.Get(x => x.IsMaster && x.Id != id).ToList().Select(x =>
            {
                dynamic newObject = new ExpandoObject();
                newObject.Name = x.TemplateName;
                newObject.Id = x.Id;
                return newObject;
            }).ToList();

            return RespondSuccess(new {
                EmailTemplate = model
            });
        }

        [Route("post")]
        [HttpPost]
        public IHttpActionResult Post(EmailTemplateEntityModel entityModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            //create a new email template
            var emailTemplate = new EmailTemplate() {
                Id = entityModel.Id,
                AdministrationEmail = entityModel.AdministrationEmail,
                EmailAccountId = entityModel.EmailAccountId,
                IsMaster = entityModel.IsMaster,
                ParentEmailTemplateId = entityModel.ParentEmailTemplateId,
                Subject = entityModel.Subject,
                Template = entityModel.Template,
                TemplateName = entityModel.TemplateName,
                TemplateSystemName = entityModel.TemplateSystemName,
                IsSystem = false
            };

            //save it
            _emailTemplateService.Insert(emailTemplate);

            VerboseReporter.ReportSuccess("Successfully saved email template", "post_emailtemplate");
            return RespondSuccess(new {
                EmailTemplate = emailTemplate.ToEntityModel()
            });

        }

        [Route("put")]
        [HttpPut]
        public IHttpActionResult Put(EmailTemplateEntityModel entityModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //get the account
            var emailTemplate = _emailTemplateService.Get(entityModel.Id);
            if (emailTemplate == null)
                return NotFound();

            emailTemplate.AdministrationEmail = entityModel.AdministrationEmail;
            emailTemplate.EmailAccountId = entityModel.EmailAccountId;
            emailTemplate.IsMaster = !emailTemplate.IsSystem && entityModel.IsMaster; //a system template can't be used as master
            emailTemplate.ParentEmailTemplateId = entityModel.ParentEmailTemplateId;
            emailTemplate.Subject = entityModel.Subject;
            emailTemplate.Template = entityModel.Template;
            emailTemplate.TemplateName = entityModel.TemplateName;
           
            //save it
            _emailTemplateService.Update(emailTemplate);
           
            VerboseReporter.ReportSuccess("Successfully updated email template", "put_emailtemplate");
            return RespondSuccess(new {
                EmailTemplate = emailTemplate.ToEntityModel()
            });

        }


        [Route("delete/{id:int}")]
        [HttpGet]
        public IHttpActionResult Delete(int id)
        {
            //get the account
            var emailTemplate = _emailTemplateService.Get(id);
            if (emailTemplate == null)
                return NotFound();

            if(emailTemplate.IsSystem)
                return RespondFailure("Can't delete a system template", "delete_emailtemplate");
            //delete the account
            _emailTemplateService.Delete(emailTemplate);

            return RespondSuccess();
        }
    }
}