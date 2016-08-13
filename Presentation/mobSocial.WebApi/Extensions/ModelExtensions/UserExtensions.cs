using System;
using System.Linq;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class UserExtensions
    {
        public static UserResponseModel ToModel(this User user)
        {
            var model = new UserResponseModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.Name,
                DateCreatedUtc = user.DateCreated,
                DateCreatedLocal = DateTimeHelper.GetDateInUserTimeZone(user.DateCreated, DateTimeKind.Utc, user),
                UserName = user.UserName,
                CoverImageUrl = user.GetPropertyValueAs<string>(PropertyNames.DefaultCoverId),
                ProfileImageUrl = user.GetPropertyValueAs<string>(PropertyNames.DefaultPictureId),
                Active = user.Active
            };

            if (!string.IsNullOrEmpty(model.CoverImageUrl) && !string.IsNullOrEmpty(model.ProfileImageUrl))
                return model;

            var mediaSettings = mobSocialEngine.ActiveEngine.Resolve<MediaSettings>();
            if (string.IsNullOrEmpty(model.CoverImageUrl))
                model.CoverImageUrl = mediaSettings.DefaultUserProfileCoverUrl;
            if (string.IsNullOrEmpty(model.ProfileImageUrl))
                model.ProfileImageUrl = mediaSettings.DefaultUserProfileImageUrl;

            return model;
        }

        public static UserEntityModel ToEntityModel(this User user)
        {
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
                LastLoginDateUtc = user.LastLoginDate,
                LastLoginDateLocal = DateTimeHelper.GetDateInUserTimeZone(user.LastLoginDate, DateTimeKind.Utc, user)
            };

            return model;
        }

    }
}