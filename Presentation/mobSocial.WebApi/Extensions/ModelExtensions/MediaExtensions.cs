using System;
using System.Linq;
using mobSocial.Core;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Helpers;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Social;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.Media;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class MediaExtensions
    {
        public static MediaReponseModel ToModel(this Media media, 
            IMediaService mediaService, 
            MediaSettings mediaSettings = null, 
            GeneralSettings generalSettings = null,
            IUserService userService = null, 
            IFollowService followService = null,
            IFriendService friendService = null,
            ICommentService commentService = null,
            ILikeService likeService = null,
            bool withUserInfo = true,
            bool withSocialInfo = false,
            bool withNextAndPreviousMedia = false)
        {
            var model = new MediaReponseModel()
            {
                Id = media.Id,
                MediaType = media.MediaType,
                Url = media.MediaType == MediaType.Image ? mediaService.GetPictureUrl(media) : mediaService.GetVideoUrl(media),
                MimeType = media.MimeType,
                DateCreatedUtc = media.DateCreated,
                ThumbnailUrl = media.MediaType == MediaType.Image ? mediaService.GetPictureUrl(media, PictureSizeNames.ThumbnailImage) : WebHelper.GetUrlFromPath(media.ThumbnailPath, generalSettings?.ImageServerDomain)
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
            if (withSocialInfo)
            {
                if (likeService != null)
                {
                    model.TotalLikes = likeService.GetLikeCount<Media>(media.Id);
                    model.LikeStatus =
                        likeService.GetCustomerLike<Media>(ApplicationContext.Current.CurrentUser.Id, media.Id) != null
                            ? 1
                            : 0;
                }

                if (commentService != null)
                {
                    model.TotalComments = commentService.GetCommentsCount(media.Id, typeof(Media).Name);
                    model.CanComment = true; //todo: perform check if comments are enabled or user has permission to comment
                }
            }

            if (withNextAndPreviousMedia)
            {
                var allMedia = mediaService.GetEntityMedia<User>(media.UserId, media.MediaType, 1, int.MaxValue).ToList();
                var mediaIndex = allMedia.FindIndex(x => x.Id == media.Id);

                model.PreviousMediaId = mediaIndex <= 0 ? 0 : allMedia[mediaIndex - 1].Id;
                model.NextMediaId = mediaIndex <= allMedia.Count - 1 ? 0 : allMedia[mediaIndex + 1].Id;
            }

            model.FullyLoaded = withSocialInfo && withNextAndPreviousMedia;
            return model;
            ;
        }
    }
}