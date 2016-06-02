using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Services.Extensions;
using mobSocial.Services.Formatter;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Sponsors;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.Sponsors;

namespace mobSocial.WebApi.Extensions
{
    public static class SponsorExtensions
    {
        public static SponsorPublicModel ToPublicModel(this Sponsor sponsor, IUserService userService, IMediaService pictureService, ISponsorService sponsorService, IFormatterService formatterService, MediaSettings mediaSettings)
        {
            var user = userService.Get(sponsor.UserId);
            if (user == null)
                return null;

            //get sponsor data
            var sponsorData = sponsorService.GetSponsorData(sponsor.BattleId, sponsor.BattleType, sponsor.UserId);

            var model = new SponsorPublicModel
            {
                SponsorshipStatus = sponsor.SponsorshipStatus,
                SponsorshipStatusName = sponsor.SponsorshipStatus.ToString(),
                CustomerId = sponsor.UserId,
                SeName = user.GetPermalink().ToString(),
                SponsorName = user.GetPropertyValueAs<string>(PropertyNames.DisplayName),
                SponsorProfileImageUrl =
                    pictureService.GetPictureUrl(user.GetPropertyValueAs<int>(PropertyNames.DefaultPictureId)),
                SponsorshipAmount = sponsor.SponsorshipAmount,
                SponsorshipAmountFormatted = formatterService.FormatCurrency(sponsor.SponsorshipAmount, ApplicationContext.Current.ActiveCurrency),
                SponsorData = sponsorData.ToModel(pictureService),
                SponsorshipType = sponsor.SponsorshipType
            };


            return model;
        }

        public static SponsorDataModel ToModel(this SponsorData sponsorData, IMediaService mediaService)
        {
            if (sponsorData == null)
                return new SponsorDataModel();
            var model = new SponsorDataModel()
            {
                Id = sponsorData.Id,
                BattleType = sponsorData.BattleType,
                BattleId = sponsorData.BattleId,
                SponsorCustomerId = sponsorData.SponsorCustomerId,
                PictureId = sponsorData.PictureId,
                DisplayName = sponsorData.DisplayName,
                TargetUrl = sponsorData.TargetUrl,
                DisplayOrder = sponsorData.DisplayOrder
            };
            if (sponsorData.PictureId > 0)
                model.PictureUrl = mediaService.GetPictureUrl(model.PictureId);

            return model;
        }
    }
}