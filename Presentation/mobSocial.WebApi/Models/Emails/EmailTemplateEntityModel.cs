using System.Collections.Generic;
using System.Web.Mvc;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Emails
{
    public class EmailTemplateEntityModel : RootEntityModel
    {
        public string TemplateName { get; set; }

        public string TemplateSystemName { get; set; }

        [AllowHtml]
        public string Template { get; set; }

        public bool IsMaster { get; set; }

        public int? ParentEmailTemplateId { get; set; }

        public int? EmailAccountId { get; set; }

        public string Subject { get; set; }

        public string AdministrationEmail { get; set; }

        public List<dynamic> AvailableMasterTemplates { get; set; }

        public List<dynamic> AvailableEmailAccounts { get; set; }
    }
}