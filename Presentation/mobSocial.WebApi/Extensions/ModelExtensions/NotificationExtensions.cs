using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Notifications;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Models.Notifications;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class NotificationExtensions
    {
        public static NotificationModel ToModel(this Notification notification)
        {
            var model = new NotificationModel()
            {
                EntityId = notification.EntityId,
                EntityName = notification.EntityName,
                EventName = notification.NotificationEvent.EventName,
                IsRead = notification.IsRead,
                PublishDateTime = notification.PublishDateTime,
                ReadDateTime = notification.ReadDateTime,
                InitiatorId = notification.InitiatorId,
            };
          

            var mediaService = mobSocialEngine.ActiveEngine.Resolve<IMediaService>();
            var mediaSettings = mobSocialEngine.ActiveEngine.Resolve<MediaSettings>();
            switch (notification.InitiatorName)
            {
                case "User":
                    var user = mobSocialEngine.ActiveEngine.Resolve<IUserService>().Get(notification.InitiatorId);
                    var pictureId = user.GetPropertyValueAs(PropertyNames.DefaultPictureId, 0);
                    model.InitiatorDisplayName = user.Name;
                    model.InitiatorImageUrl = pictureId != 0 ? mediaService.GetPictureUrl(pictureId) : mediaSettings.DefaultUserProfileImageUrl;
                    break;
            }

            return model;
        }
    }
}