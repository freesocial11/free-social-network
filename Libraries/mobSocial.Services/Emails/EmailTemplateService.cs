using mobSocial.Core.Data;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public class EmailTemplateService : MobSocialEntityService<EmailTemplate>, IEmailTemplateService
    {
        public EmailTemplateService(IDataRepository<EmailTemplate> dataRepository) : base(dataRepository)
        {

        }
    }
}