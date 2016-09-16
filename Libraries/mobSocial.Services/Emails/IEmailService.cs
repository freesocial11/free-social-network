using mobSocial.Core.Services;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public interface IEmailService : IBaseEntityService<EmailMessage>
    {
        /// <summary>
        /// Sends an email with settings specified in the email info object and returns true if sending succeeds
        /// </summary>
        bool SendEmail(EmailMessage emailMessage, bool verboseErrorOnFailure = false);

        void Queue(EmailMessage emailMessage);
    }
}