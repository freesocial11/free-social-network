using System;
using System.Linq;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class UserExtensions
    {
        public static UserResponseModel ToModel(this User user, IMediaService mediaService, MediaSettings mediaSettings)
        {
            var model = new UserResponseModel() {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.Name,
                DateCreatedUtc = user.DateCreated,
                DateCreatedLocal = DateTimeHelper.GetDateInUserTimeZone(user.DateCreated, DateTimeKind.Utc, user),
                UserName = user.UserName,
                CoverImageUrl = mediaService.GetPictureUrl(user.GetPropertyValueAs<int>(PropertyNames.DefaultCoverId), PictureSizeNames.MediumCover),
                ProfileImageUrl = mediaService.GetPictureUrl(user.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId), PictureSizeNames.MediumProfileImage),
                Active = user.Active
            };
            //TODO: Put capability check instead of administration check, that'd be more scalable
            if (ApplicationContext.Current.CurrentUser.IsAdministrator() && user.LastLoginDate.HasValue)
            {
                model.LastLoginDateUtc = user.LastLoginDate;
                model.LastLoginDateLocal = DateTimeHelper.GetDateInUserTimeZone(user.LastLoginDate.Value,
                    DateTimeKind.Utc, user);
            }

            if (!string.IsNullOrEmpty(model.CoverImageUrl) && !string.IsNullOrEmpty(model.ProfileImageUrl))
                return model;

            if (string.IsNullOrEmpty(model.CoverImageUrl))
                model.CoverImageUrl = mediaSettings.DefaultUserProfileCoverUrl;
            if (string.IsNullOrEmpty(model.ProfileImageUrl))
                model.ProfileImageUrl = mediaSettings.DefaultUserProfileImageUrl;

            return model;
        }

        public static UserEntityModel ToEntityModel(this User user, IMediaService mediaService, MediaSettings mediaSettings)
        {
            var userCoverId = user.GetPropertyValueAs<int>(PropertyNames.DefaultCoverId);
            var userProfileImageId = user.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId);
            var model = new UserEntityModel() {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Active = user.Active,
                Remarks = user.Remarks,
                RoleIds = user.UserRoles.Select(x => x.RoleId).ToList(),
                CoverImageId = userCoverId,
                ProfileImageId = userProfileImageId
            };
            //TODO: Put capability check instead of administration check, that'd be more scalable
            if (ApplicationContext.Current.CurrentUser.IsAdministrator() && user.LastLoginDate.HasValue)
            {
                model.LastLoginDateUtc = user.LastLoginDate;
                model.LastLoginDateLocal = DateTimeHelper.GetDateInUserTimeZone(user.LastLoginDate.Value,
                    DateTimeKind.Utc, user);
            }
            model.CoverImageUrl = userCoverId == 0 ? mediaSettings.DefaultUserProfileCoverUrl : mediaService.GetPictureUrl(userCoverId, PictureSizeNames.MediumCover);
            model.ProfileImageUrl = userProfileImageId == 0 ? mediaSettings.DefaultUserProfileImageUrl : mediaService.GetPictureUrl(userProfileImageId, PictureSizeNames.MediumProfileImage);
            return model;
        }

    }
}