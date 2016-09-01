using System;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Models.Media;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class MediaExtensions
    {
        public static MediaReponseModel ToModel(this Media media, 
            IMediaService mediaService, 
            MediaSettings mediaSettings = null, 
            IUserService userService = null, 
            IFollowService followService = null,
            IFriendService friendService = null,
            bool withUserInfo = true)
        {
            var model = new MediaReponseModel()
            {
                Id = media.Id,
                MediaType = media.MediaType,
                Url = media.MediaType == MediaType.Image ? mediaService.GetPictureUrl(media) : mediaService.GetVideoUrl(media),
                DateCreatedUtc = media.DateCreated
            };
            if (withUserInfo && userService != null)
            {
                var user = userService.Get(media.UserId);
                if (user != null)
                {
                    model.CreatedBy = user.ToModel(mediaService, mediaSettings, followService, friendService);
                    model.DateCreatedLocal = DateTimeHelper.GetDateInUserTimeZone(media.DateCreated, DateTimeKind.Utc, user);
                }
            }
            return model;
        }
    }
}