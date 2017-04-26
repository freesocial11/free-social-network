using System;
using System.Collections.Generic;
using mobSocial.Data.Entity.GroupPages;
using mobSocial.Data.Entity.TeamPages;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.TeamPages;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class TeamPageExtensions
    {
        public static TeamPageModel ToEntityModel(this TeamPage teamPage, IMediaService mediaService)
        {
            var model = new TeamPageModel()
            {
                Id = teamPage.Id,
                TeamPictureUrl = mediaService.GetPictureUrl(teamPage.TeamPictureId),
                Name = teamPage.Name,
                Description = teamPage.Description,
                CreatedBy = teamPage.CreatedBy,
                CreatedOn = teamPage.CreatedOn,
                UpdatedBy = teamPage.UpdatedBy,
                UpdatedOn = teamPage.UpdatedOn
            };
            return model;
        }

        public static TeamPagePublicModel ToModel(this TeamPage teamPage, IMediaService mediaService)
        {
            var model = new TeamPagePublicModel()
            {
                Name = teamPage.Name,
                Description = teamPage.Description,
                CreatedOn = teamPage.CreatedOn,
                UpdatedOn = teamPage.UpdatedOn,
                TeamPictureUrl = mediaService.GetPictureUrl(teamPage.TeamPictureId),
                Id = teamPage.Id
            };
            return model;
        }

        public static TeamPageGroupPublicModel ToModel(this GroupPage groupPage)
        {
            var groupModel = new TeamPageGroupPublicModel() {
                CreatedOnUtc = groupPage.DateCreated,
                CreatedOn = DateTimeHelper.GetDateInUserTimeZone(groupPage.DateCreated, DateTimeKind.Utc, ApplicationContext.Current.CurrentUser),
                UpdatedOnUtc = groupPage.DateUpdated,
                UpdatedOn = DateTimeHelper.GetDateInUserTimeZone(groupPage.DateUpdated, DateTimeKind.Utc, ApplicationContext.Current.CurrentUser),
                Id = groupPage.Id,
                DisplayOrder = groupPage.DisplayOrder,
                IsDefault = groupPage.IsDefault,
                PaypalDonateUrl = groupPage.PayPalDonateUrl,
                TeamPageId = groupPage.TeamPageId,
                Name = groupPage.Name,
                Description = groupPage.Description,
                GroupMembers = new List<UserResponseModel>()
            };
            return groupModel;
        }
    }
}