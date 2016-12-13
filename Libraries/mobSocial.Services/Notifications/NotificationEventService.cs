using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Notifications;

namespace mobSocial.Services.Notifications
{
    public class NotificationEventService : BaseEntityService<NotificationEvent>, INotificationEventService
    {
        public NotificationEventService(IDataRepository<NotificationEvent> dataRepository) : base(dataRepository)
        {
        }
    }
}