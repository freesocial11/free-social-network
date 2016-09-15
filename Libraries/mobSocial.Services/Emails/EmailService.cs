using System;
using System.Net;
using System.Net.Mail;
using mobSocial.Core.Data;
using mobSocial.Core.Exception;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Emails;
using mobSocial.Services.Security;

namespace mobSocial.Services.Emails
{
    public class EmailService : BaseEntityService<EmailMessage>, IEmailService
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly ICryptographyService _cryptographyService;

        public EmailService(IDataRepository<EmailMessage> dataRepository, IEmailAccountService emailAccountService, ICryptographyService cryptographyService) : base(dataRepository)
        {
            _emailAccountService = emailAccountService;
            _cryptographyService = cryptographyService;
        }

        public bool SendEmail(EmailMessage emailMessage)
        {
            //we need an email account
            var emailAccount = emailMessage.EmailAccount ?? _emailAccountService.FirstOrDefault(x => x.IsDefault);
            if (emailAccount == null)
                return false; //can't send email without account

            var message = new MailMessage();
            //from, to, reply to
            message.From = new MailAddress(emailAccount.Email, emailAccount.FromName);

            if (emailMessage.Tos == null && emailMessage.Ccs == null && emailMessage.Bccs == null)
            {
                throw new mobSocialException("At least one of Tos, CCs or BCCs must be specified to send email");
            }

            if (emailMessage.Tos != null)
                foreach (var userInfo in emailMessage.Tos)
                {
                    message.To.Add(new MailAddress(userInfo.Email, userInfo.Name));
                }

            if (emailMessage.ReplyTos != null)
                foreach (var userInfo in emailMessage.ReplyTos)
                {
                    message.ReplyToList.Add(new MailAddress(userInfo.Email, userInfo.Name));
                }

            if (emailMessage.Bccs != null)
                foreach (var userInfo in emailMessage.Bccs)
                {
                    message.Bcc.Add(new MailAddress(userInfo.Email, userInfo.Name));
                }

            if (emailMessage.Ccs != null)
                foreach (var userInfo in emailMessage.Ccs)
                {
                    message.Bcc.Add(new MailAddress(userInfo.Email, userInfo.Name));
                }

            //content
            message.Subject = emailMessage.Subject;
            message.Body = emailMessage.EmailBody;
            message.IsBodyHtml = emailMessage.IsEmailBodyHtml;

            //headers
            if (emailMessage.Headers != null)
                foreach (var header in emailMessage.Headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

            if (emailMessage.Attachments != null)
                foreach (var attachment in emailMessage.Attachments)
                    message.Attachments.Add(attachment);

            //send email

            var password = _cryptographyService.Decrypt(emailAccount.Password);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.UseSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(emailAccount.UserName, password);
                try
                {
                    smtpClient.Send(message);
                    //update the send status
                    emailMessage.IsSent = true;
                    Update(emailMessage);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public void Queue(EmailMessage emailMessage)
        {
            Insert(emailMessage);
        }
    }
}
