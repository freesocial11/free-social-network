using System;
using System.Linq;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Tasks;
using mobSocial.Services.Emails;

namespace mobSocial.WebApi.Configuration.Tasks
{
    public class EmailSchedulerTask : ITask
    {
        public void Dispose()
        {
            //do nothing else
        }

        public void Run()
        {
            //resolve email sender service
            var emailService = mobSocialEngine.ActiveEngine.Resolve<IEmailService>();
            var emailMessages = emailService.Get(x => !x.IsSent && x.SendingDate <= DateTime.UtcNow).ToList();

            foreach (var message in emailMessages)
                emailService.SendEmail(message);
        }

        public string SystemName => "mobSocial.WebApi.Configuration.Tasks.EmailSchedulerTask";
    }
}