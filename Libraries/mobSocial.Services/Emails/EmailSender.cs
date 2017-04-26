using System;
using System.Collections.Generic;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Entity.Emails;
using mobSocial.Data.Entity.Tokens;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Tokens;
using mobSocial.Services.Users;

namespace mobSocial.Services.Emails
{
    public class EmailSender : IEmailSender
    {
        #region fields
        private readonly IEmailService _emailService;
        private readonly ITokenProcessor _tokenProcessor;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IUserService _userService;
        #endregion

        public EmailSender(IEmailService emailService, ITokenProcessor tokenProcessor, IEmailTemplateService emailTemplateService, IUserService userService)
        {
            _emailService = emailService;
            _tokenProcessor = tokenProcessor;
            _emailTemplateService = emailTemplateService;
            _userService = userService;
        }
        /// <summary>
        /// Loads a named email template from database and replaces tokens with passed entities, and returns a new email message object with template values
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        private EmailMessage LoadAndProcessTemplate(string templateName, params object[] entities)
        {
            //first load the template from database
            var template = _emailTemplateService.FirstOrDefault(x => x.TemplateSystemName == templateName, x => x.EmailAccount, x => x.ParentEmailTemplate);
            if (template == null)
                return null;
            //we'll check if there are parent templates and get all parent template 
            var processedContentTemplate = _emailTemplateService.GetProcessedContentTemplate(template);

            var processedTemplateString = _tokenProcessor.ProcessAllTokens(processedContentTemplate, entities);
            var emailAccount = template.EmailAccount;
            //create a new email message
            var emailMessage = new EmailMessage() {
                IsEmailBodyHtml = true,
                EmailBody = processedTemplateString,
                EmailAccount = emailAccount,
                Subject = template.Subject,
                OriginalEmailTemplate = template,
                Tos = new List<EmailMessage.UserInfo>(),
                SendingDate = DateTime.UtcNow
            };

            return emailMessage;
        }


        public bool SendTestEmail(string email, EmailAccount emailAccount)
        {
            var subject = "MobSocial Test Email";
            var message = "Thank you for using mobSocial. This is a sample email to test if emails are functioning.";
            //create a new email message
            var emailMessage = new EmailMessage() {
                IsEmailBodyHtml = true,
                EmailBody = message,
                EmailAccount = emailAccount,
                Subject = subject,
                Tos = new List<EmailMessage.UserInfo>()
                {
                    new EmailMessage.UserInfo("WebAdmin", email)
                }
            };
            return _emailService.SendEmail(emailMessage, true);
        }

        public void SendUserRegisteredMessage(User user, bool withAdmin = true)
        {
            var message = LoadAndProcessTemplate(EmailTemplateNames.UserRegisteredMessage, user);
            message.Tos.Add(new EmailMessage.UserInfo(user.Name, user.Email));
             _emailService.Queue(message);
            if (withAdmin) //send to admin if needed
            {
                message = LoadAndProcessTemplate(EmailTemplateNames.UserRegisteredMessageToAdmin, user);
                if (message != null)
                {
                    message.Tos.Add(new EmailMessage.UserInfo("Administrator", message.OriginalEmailTemplate.AdministrationEmail));
                    _emailService.Queue(message);
                }
                
            }
        }

        public void SendUserActivationLinkMessage(User user, string activationUrl)
        {
            var message = LoadAndProcessTemplate(EmailTemplateNames.UserActivationLinkMessage, user);
            //additional tokens 
            message.EmailBody = _tokenProcessor.ProcessProvidedTokens(message.EmailBody, new List<Token>
            {
                new Token(EmailTokenNames.ActivationUrl, activationUrl)
            });
            message.Tos.Add(new EmailMessage.UserInfo(user.Name, user.Email));
            _emailService.Queue(message);
        }

        public void SendUserActivatedMessage(User user)
        {
            var message = LoadAndProcessTemplate(EmailTemplateNames.UserActivatedMessage, user);
            message.Tos.Add(new EmailMessage.UserInfo(user.Name, user.Email));
            _emailService.Queue(message);
        }

        public void SendFriendRequestNotification(User user, int friendRequestCount)
        {
            throw new System.NotImplementedException();
        }

        public void SendEventInvitationNotification(User user)
        {
            var message = LoadAndProcessTemplate(EmailTemplateNames.EventInvite, user);
            message.Tos.Add(new EmailMessage.UserInfo(user.Name, user.Email));
            _emailService.Queue(message);
        }

        public void SendPendingFriendRequestNotification(User user, int friendRequestCount)
        {
            throw new System.NotImplementedException();
        }

        public void SendBirthdayNotification(User user)
        {
            throw new System.NotImplementedException();
        }

        public void SendSomeoneSentYouASongNotification(User userUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendSomeoneChallengedYouForABattleNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendSomeoneChallengedYouForABattleNotification(User challenger, string challengeeEmail, string challengeeName,
            VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleCompleteNotification(User user, VideoBattle videoBattle, NotificationRecipientType recipientTypeUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVotingReminderNotification(User sender, User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVotingReminderNotification(User sender, string receiverEmail, string receiverName, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleSignupNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleJoinNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleSignupAcceptedNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleDisqualifiedNotification(User challenger, User challengee, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendVideoBattleOpenNotification(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendSponsorAppliedNotificationToBattleOwner(User owner, User sponsor, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendSponsorshipStatusChangeNotification(User receiver, SponsorshipStatus sponsorshipStatus,
            VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendXDaysToBattleStartNotificationToParticipant(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }

        public void SendXDaysToBattleEndNotificationToFollower(User receiver, VideoBattle videoBattleUser)
        {
            throw new System.NotImplementedException();
        }
    }
}