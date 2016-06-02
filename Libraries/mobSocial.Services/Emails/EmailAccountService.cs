using mobSocial.Core.Data;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public class EmailAccountService : MobSocialEntityService<EmailAccount>, IEmailAccountService
    {
        public EmailAccountService(IDataRepository<EmailAccount> dataRepository) : base(dataRepository)
        {
        }
    }
}