using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Emails;
using mobSocial.Services.Emails;
using mobSocial.Services.Security;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Emails;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("emailaccounts")]
    [AdminAuthorize]
    public class EmailAccountController : RootApiController
    {
        #region variables
        private readonly IEmailAccountService _emailAccountService;
        private readonly ICryptographyService _cryptographyService;
        #endregion

        #region ctor
        public EmailAccountController(IEmailAccountService emailAccountService, ICryptographyService cryptographyService)
        {
            _emailAccountService = emailAccountService;
            _cryptographyService = cryptographyService;
        }

        #endregion

        [Route("get")]
        [HttpGet]
        public IHttpActionResult Get([FromUri] EmailAccountGetModel requestModel)
        {
            if (requestModel.Count <= 0)
                requestModel.Count = 15;

            if (requestModel.Page <= 0)
                requestModel.Page = 1;

            //get the email accounts
            var emailAccounts = _emailAccountService.Get(page: requestModel.Page, count: requestModel.Count).ToList();
            var model = emailAccounts.Select(x => x.ToEntityModel());
            return RespondSuccess(new
            {
                EmailAccounts = model
            });
        }

        [Route("get/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            //get the account
            var emailAccount = _emailAccountService.Get(id);
            if(emailAccount == null)
                return NotFound();
            return RespondSuccess(new {
                EmailAccount = emailAccount.ToEntityModel()
            });
        }

        [Route("post")]
        [HttpPost]
        public IHttpActionResult Post(EmailAccountEntityModel entityModel)
        {
           //create a new email account
            var emailAccount = new EmailAccount()
            {
                Id = entityModel.Id,
                Email = entityModel.Email,
                UserName = entityModel.UserName,
                FromName = entityModel.FromName,
                Host = entityModel.Host,
                UseSsl = entityModel.UseSsl,
                Port = entityModel.Port,
                UseDefaultCredentials = entityModel.UseDefaultCredentials,
                Password = _cryptographyService.Encrypt(entityModel.Password)
            };

            //if this is the first account, we'll set it as default
            emailAccount.IsDefault = _emailAccountService.Count() == 0;

            //save it
            _emailAccountService.Insert(emailAccount);

            VerboseReporter.ReportSuccess("Successfully saved email account", "post_emailaccount");
            return RespondSuccess(new {
                EmailAccount = emailAccount.ToEntityModel()
            });

        }

        [Route("put")]
        [HttpPut]
        public IHttpActionResult Put(EmailAccountEntityModel entityModel)
        {

            //get the account
            var emailAccount = _emailAccountService.Get(entityModel.Id);
            if (emailAccount == null)
                return NotFound();

            emailAccount.FromName = entityModel.FromName;
            emailAccount.Host = entityModel.Host;
            emailAccount.IsDefault = entityModel.IsDefault;
            emailAccount.Port = entityModel.Port;
            emailAccount.UseDefaultCredentials = entityModel.UseDefaultCredentials;
            emailAccount.UseSsl = entityModel.UseSsl;
            emailAccount.UserName = entityModel.UserName;
            emailAccount.Email = entityModel.Email;

            //we'll change password if it's provided
            if (!string.IsNullOrEmpty(entityModel.Password))
            {
                emailAccount.Password = _cryptographyService.Encrypt(entityModel.Password);
            }
            //save it
            _emailAccountService.Update(emailAccount);

            //if this is a default account now, update the previous one
            if (emailAccount.IsDefault)
            {
                var oldDefaultEmailAccount = _emailAccountService.FirstOrDefault(x => x.IsDefault && x.Id != emailAccount.Id);
                if (oldDefaultEmailAccount != null)
                {
                    oldDefaultEmailAccount.IsDefault = false;
                    _emailAccountService.Update(emailAccount);
                }
            }
            VerboseReporter.ReportSuccess("Successfully updated email account", "put_emailaccount");
            return RespondSuccess(new {
                EmailAccount = emailAccount.ToEntityModel()
            });

        }


        [Route("delete/{id:int}")]
        [HttpGet]
        public IHttpActionResult Delete(int id)
        {
            //get the account
            var emailAccount = _emailAccountService.Get(id);
            if (emailAccount == null)
                return NotFound();

            //delete the account
            _emailAccountService.Delete(emailAccount);

            return RespondSuccess();
        }
    }
}