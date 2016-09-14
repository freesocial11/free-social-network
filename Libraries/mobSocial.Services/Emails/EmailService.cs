using System;
using System.Net;
using System.Net.Mail;
using mobSocial.Core.Data;
using mobSocial.Core.Exception;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Emails;

namespace mobSocial.Services.Emails
{
    public class EmailService : BaseEntityService<EmailMessage>, IEmailService
    {
        public EmailService(IDataRepository<EmailMessage> dataRepository) : base(dataRepository)
        {
        }

        public bool SendEmail(EmailMessage emailMessage)
        {
            var message = new MailMessage();
            //from, to, reply to
            message.From = new MailAddress(emailMessage.FromEmail, emailMessage.FromName);

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
            var emailAccount = emailMessage.EmailAccount;
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.UseSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(emailAccount.UserName, emailAccount.Password);
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

        
    }
}
