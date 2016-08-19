using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using mobSocial.Core;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Models.Settings;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class SettingExtensions
    {
        public static GeneralSettingsModel ToModel(this GeneralSettings generalSettings)
        {
            var model = new GeneralSettingsModel() {
                ApplicationApiRoot = generalSettings.ApplicationApiRoot,
                VideoServerDomain = generalSettings.VideoServerDomain,
                ApplicationUiDomain = generalSettings.ApplicationUiDomain,
                ApplicationCookieDomain = generalSettings.ApplicationCookieDomain,
                ImageServerDomain = generalSettings.ImageServerDomain,
                DefaultTimeZoneId = generalSettings.DefaultTimeZoneId,
                AvailableTimeZones = ServerHelper.GetAvailableTimezones().Select(x =>
                {
                    dynamic tzObject = new ExpandoObject();
                    tzObject.Id = x.Id;
                    tzObject.DisplayName = x.DisplayName;
                    return tzObject;

                }).ToList()
            };
            return model;
        }

        public static BattleSettingsModel ToModel(this BattleSettings battleSettings)
        {
            var model = new BattleSettingsModel() {
                BattleHostPictureBattleSponsorshipPercentage = battleSettings.BattleHostPictureBattleSponsorshipPercentage,
                BattleHostVideoBattleSponsorshipPercentage = battleSettings.BattleHostVideoBattleSponsorshipPercentage,
                DefaultVideosFeaturedImageUrl = battleSettings.DefaultVideosFeaturedImageUrl,
                DefaultVotingChargeForPaidVoting = battleSettings.DefaultVotingChargeForPaidVoting,
                SiteOwnerPictureBattleSponsorshipPercentage = battleSettings.SiteOwnerPictureBattleSponsorshipPercentage,
                SiteOwnerVideoBattleSponsorshipPercentage = battleSettings.SiteOwnerVideoBattleSponsorshipPercentage
            };
            return model;
        }

        public static DateTimeSettingsModel ToModel(this DateTimeSettings dateTimeSettings)
        {
            var model = new DateTimeSettingsModel() {
                DefaultTimeZoneId = dateTimeSettings.DefaultTimeZoneId
            };
            return model;
        }

        public static SecuritySettingsModel ToModel(this SecuritySettings securitySettings)
        {
            var model = new SecuritySettingsModel()
            {
                DefaultPasswordStorageFormat = securitySettings.DefaultPasswordStorageFormat,
                AvailablePasswordStorageFormats = new List<dynamic>()
                {
                    new { Value = PasswordFormat.Md5Hashed, DisplayText = "MD5" },
                    new { Value = PasswordFormat.Sha1Hashed, DisplayText = "SHA1" },
                    new { Value = PasswordFormat.Sha256Hashed, DisplayText = "SHA256" }

                }
            };
            return model;
        }

        public static ThirdPartySettingsModel ToModel(this ThirdPartySettings thirdPartySettings)
        {
            var model = new ThirdPartySettingsModel()
            {
                EchonestApiKey = thirdPartySettings.EchonestApiKey,
                SevenDigitalOAuthConsumerKey = thirdPartySettings.SevenDigitalOAuthConsumerKey,
                SevenDigitalOAuthConsumerSecret = thirdPartySettings.SevenDigitalOAuthConsumerSecret,
                SevenDigitalPartnerId = thirdPartySettings.SevenDigitalPartnerId
            };
            return model;
        }

        public static MediaSettingsModel ToModel(this MediaSettings mediaSettings)
        {
            var model = new MediaSettingsModel()
            {
                SmallProfilePictureSize = mediaSettings.SmallProfilePictureSize,
                PictureSavePath = mediaSettings.PictureSavePath,
                SmallCoverPictureSize = mediaSettings.SmallCoverPictureSize,
                MediumCoverPictureSize = mediaSettings.MediumCoverPictureSize,
                OtherMediaSavePath = mediaSettings.OtherMediaSavePath,
                MediumProfilePictureSize = mediaSettings.MediumProfilePictureSize,
                DefaultUserProfileImageUrl = mediaSettings.DefaultUserProfileImageUrl,
                PictureSaveLocation = mediaSettings.PictureSaveLocation,
                DefaultUserProfileCoverUrl = mediaSettings.DefaultUserProfileCoverUrl,
                VideoSavePath = mediaSettings.VideoSavePath,
                OtherMediaSaveLocation = mediaSettings.OtherMediaSaveLocation,
                ThumbnailPictureSize = mediaSettings.ThumbnailPictureSize,
                MaximumFileUploadSizeForDocuments = mediaSettings.MaximumFileUploadSizeForDocuments,
                MaximumFileUploadSizeForImages = mediaSettings.MaximumFileUploadSizeForImages,
                MaximumFileUploadSizeForVideos = mediaSettings.MaximumFileUploadSizeForVideos
            };
            return model;
        }

        public static UserSettingsModel ToModel(this UserSettings userSettings)
        {
            var model = new UserSettingsModel()
            {
                AreUserNamesEnabled = userSettings.AreUserNamesEnabled,
                UserRegistrationDefaultMode = userSettings.UserRegistrationDefaultMode,
                AvailableUserRegistrationModes = new List<dynamic>()
                {
                    new { Value = RegistrationMode.Immediate, DisplayText = "Immediate" },
                    new { Value = RegistrationMode.WithActivationEmail, DisplayText = "Send activation email to complete registration" },
                    new { Value = RegistrationMode.ManualApproval, DisplayText = "Manually approve the registration" }

                }
            };
            return model;
        }

        public static PaymentSettingsModel ToModel(this PaymentSettings paymentSettings)
        {
            var model = new PaymentSettingsModel()
            {
                CreditExchangeRate = paymentSettings.CreditExchangeRate,
                IsPromotionalCreditUsageLimitPercentage = paymentSettings.IsPromotionalCreditUsageLimitPercentage,
                MacroPaymentsFixedPaymentProcessingFee = paymentSettings.MacroPaymentsFixedPaymentProcessingFee,
                MacroPaymentsPaymentProcessingPercentage = paymentSettings.MacroPaymentsPaymentProcessingPercentage,
                MicroPaymentsFixedPaymentProcessingFee = paymentSettings.MicroPaymentsFixedPaymentProcessingFee,
                MicroPaymentsPaymentProcessingPercentage = paymentSettings.MicroPaymentsPaymentProcessingPercentage,
                PaymentMethodSelectionType = paymentSettings.PaymentMethodSelectionType,
                PromotionalCreditUsageLimitPerTransaction = paymentSettings.PromotionalCreditUsageLimitPerTransaction
            };
            return model;
        }

    }
}