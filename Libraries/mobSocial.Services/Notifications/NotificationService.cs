using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Notifications;

namespace mobSocial.Services.Notifications
{
    public class NotificationService : BaseEntityService<Notification>, INotificationService
    {
        public NotificationService(IDataRepository<Notification> dataRepository) : base(dataRepository)
        {
        }
    }

}
