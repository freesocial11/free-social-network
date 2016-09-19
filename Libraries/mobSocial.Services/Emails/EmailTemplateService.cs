using System.Text;
using mobSocial.Core.Data;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public class EmailTemplateService : MobSocialEntityService<EmailTemplate>, IEmailTemplateService
    {
        public EmailTemplateService(IDataRepository<EmailTemplate> dataRepository) : base(dataRepository)
        {

        }

        public string GetProcessedContentTemplate(EmailTemplate emailTemplate)
        {
            if (emailTemplate == null) return "";

            if (emailTemplate.ParentEmailTemplate == null)
                return emailTemplate.Template;
            return
                GetProcessedContentTemplate(emailTemplate.ParentEmailTemplate)
                    .Replace(EmailTokenNames.MessageContent, emailTemplate.Template);
        }
    }
}