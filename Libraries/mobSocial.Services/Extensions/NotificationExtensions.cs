using System;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.Notifications;
using mobSocial.Services.Notifications;

namespace mobSocial.Services.Extensions
{
    public static class NotificationExtensions
    {
        public static void Notify<T>(this INotificationService notificationService, int userId, string eventName, T entity, string initiatorName, int initiatorId, DateTime publishDate) where T : BaseEntity
        {
            var notificationEventService = mobSocialEngine.ActiveEngine.Resolve<INotificationEventService>();
            var notificationEvent = notificationEventService.FirstOrDefault(x => x.EventName == eventName);
            if (notificationEvent != null && !notificationEvent.Enabled)
                return;

            var notification = new Notification() {
                EntityId = entity.Id,
                EntityName = typeof(T).Name,
                IsRead = false,
                UserId = userId,
                PublishDateTime = publishDate,
                ReadDateTime = null,
                NotificationEventId = notificationEvent?.Id,
                InitiatorId = initiatorId,
                InitiatorName = initiatorName
            };
            notificationService.Insert(notification, true);
        }

        public static void DeNotify<T>(this INotificationService notificationService, int userId, string eventName,
            int entityId, string initiatorName, int initiatorId) where T : BaseEntity
        {
            var notificationEventService = mobSocialEngine.ActiveEngine.Resolve<INotificationEventService>();
            var notificationEvent = notificationEventService.FirstOrDefault(x => x.EventName == eventName);
            if (notificationEvent == null)
                return;
            notificationService.Delete(x => x.EntityId == entityId 
            && x.EntityName == typeof(T).Name 
            && x.InitiatorId == initiatorId
            && x.InitiatorName == initiatorName
            && x.NotificationEventId == notificationEvent.Id 
            && x.UserId == userId);
        }
        public static void MarkRead(this INotificationService notificationService,  int notificationId)
        {
            var notification = notificationService.Get(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadDateTime = DateTime.UtcNow;
                notificationService.Update(notification);
            }
        }
    }
}