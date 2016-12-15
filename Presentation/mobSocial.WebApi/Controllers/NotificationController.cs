using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Notifications;
using mobSocial.Services.Extensions;
using mobSocial.Services.Notifications;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("notifications")]
    public class NotificationController : RootApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [Route("put")]
        [HttpPut]
        [Authorize]
        public IHttpActionResult MarkNotificationsRead(int[] notificationIds)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            IList<Notification> notifications;
            if (notificationIds == null || !notificationIds.Any())
            {
                //we'll be marking all unread notifications as read
                notifications = _notificationService.Get(x => !x.IsRead && x.UserId == currentUser.Id).ToList();
            }
            else
            {
                notifications = _notificationService.Get(x => notificationIds.Contains(x.Id) && x.UserId == currentUser.Id).ToList();
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDateTime = DateTime.UtcNow;
                _notificationService.Update(notification);
            }
            return RespondSuccess();
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var notification = _notificationService.Get(id);
            if (notification.UserId != currentUser.Id && !currentUser.IsAdministrator())
            {
                return Unauthorized();
            }

            //delete the notification
            _notificationService.Delete(notification);
            return RespondSuccess();
        }
    }
}