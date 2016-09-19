using mobSocial.Core.Services;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public interface IEmailTemplateService : IBaseEntityService<EmailTemplate>
    {
        string GetProcessedContentTemplate(EmailTemplate emailTemplate);
    }
}