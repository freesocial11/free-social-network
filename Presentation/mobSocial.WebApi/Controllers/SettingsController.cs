using System.Web.Http;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Settings;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Settings;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("settings")]
    public class SettingsController : RootApiController
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [Route("get/{settingType}")]
        [AdminAuthorize]
        [HttpGet]
        public IHttpActionResult Get(string settingType = "all")
        {
            GeneralSettings generalSettings;
            MediaSettings mediaSettings;
            UserSettings userSettings;
            ThirdPartySettings thirdPartySettings;
            SecuritySettings securitySettings;
            DateTimeSettings dateTimeSettings;
            PaymentSettings paymentSettings;
            BattleSettings battleSettings;
            UrlSettings urlSettings;
            switch (settingType)
            {
                case "general":
                    generalSettings = _settingService.GetSettings<GeneralSettings>();
                    return RespondSuccess(new
                    {
                        GeneralSettings = generalSettings.ToModel()
                    });
                case "media":
                    mediaSettings = _settingService.GetSettings<MediaSettings>();
                    return RespondSuccess(new {
                        MediaSettings = mediaSettings.ToModel()
                    });
                case "user":
                    userSettings = _settingService.GetSettings<UserSettings>();
                    return RespondSuccess(new {
                        UserSettings = userSettings.ToModel()
                    });
                case "security":
                    securitySettings = _settingService.GetSettings<SecuritySettings>();
                    return RespondSuccess(new {
                        SecuritySettings = securitySettings.ToModel()
                    });
                case "battle":
                    battleSettings = _settingService.GetSettings<BattleSettings>();
                    return RespondSuccess(new {
                        BattleSettings = battleSettings.ToModel()
                    });
                case "datetime":
                    dateTimeSettings = _settingService.GetSettings<DateTimeSettings>();
                    return RespondSuccess(new {
                        DateTimeSettings = dateTimeSettings.ToModel()
                    });
                case "payment":
                    paymentSettings = _settingService.GetSettings<PaymentSettings>();
                    return RespondSuccess(new {
                        PaymentSettings = paymentSettings.ToModel()
                    });
                case "thirdparty":
                    thirdPartySettings = _settingService.GetSettings<ThirdPartySettings>();
                    return RespondSuccess(new {
                        ThirdPartySettings = thirdPartySettings.ToModel()
                    });
                case "url":
                    urlSettings = _settingService.GetSettings<UrlSettings>();
                    return RespondSuccess(new {
                        UrlSettings = urlSettings.ToModel()
                    });
                case "all":
                    generalSettings = _settingService.GetSettings<GeneralSettings>();
                    mediaSettings = _settingService.GetSettings<MediaSettings>();
                    userSettings = _settingService.GetSettings<UserSettings>();
                    securitySettings = _settingService.GetSettings<SecuritySettings>();
                    battleSettings = _settingService.GetSettings<BattleSettings>();
                    dateTimeSettings = _settingService.GetSettings<DateTimeSettings>();
                    paymentSettings = _settingService.GetSettings<PaymentSettings>();
                    thirdPartySettings = _settingService.GetSettings<ThirdPartySettings>();
                    urlSettings = _settingService.GetSettings<UrlSettings>();
                    return RespondSuccess(new
                    {
                        GeneralSettings = generalSettings?.ToModel(),
                        MediaSettings = mediaSettings.ToModel(),
                        UserSettings = userSettings.ToModel(),
                        SecuritySettings = securitySettings.ToModel(),
                        BattleSettings = battleSettings.ToModel(),
                        DateTimeSettings = dateTimeSettings.ToModel(),
                        PaymentSettings = paymentSettings.ToModel(),
                        ThirdPartySettings = thirdPartySettings.ToModel(),
                        UrlSettings = urlSettings.ToModel()
                    });
                default:
                    VerboseReporter.ReportError("Invalid type name", "get_setting");
                    return RespondFailure();
            }
        }
        [Route("post/general")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(GeneralSettingsModel entityModel)
        {
            var generalSettings = new GeneralSettings()
            {
                ApplicationApiRoot = entityModel.ApplicationApiRoot,
                ApplicationCookieDomain = entityModel.ApplicationCookieDomain,
                ApplicationUiDomain = entityModel.ApplicationUiDomain,
                VideoServerDomain = entityModel.VideoServerDomain,
                ImageServerDomain = entityModel.ImageServerDomain,
                DefaultTimeZoneId = entityModel.DefaultTimeZoneId
            };
            //save it and respond
            _settingService.Save(generalSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { GeneralSettings = generalSettings.ToModel() });
        }

        [Route("post/media")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(MediaSettingsModel entityModel)
        {
            var mediaSettings = new MediaSettings()
            {
                SmallCoverPictureSize = entityModel.SmallCoverPictureSize,
                MediumCoverPictureSize = entityModel.MediumCoverPictureSize,
                OtherMediaSavePath = entityModel.OtherMediaSavePath,
                PictureSavePath = entityModel.PictureSavePath,
                VideoSavePath = entityModel.VideoSavePath,
                MediumProfilePictureSize = entityModel.MediumProfilePictureSize,
                SmallProfilePictureSize = entityModel.SmallProfilePictureSize,
                DefaultUserProfileImageUrl = entityModel.DefaultUserProfileImageUrl,
                DefaultUserProfileCoverUrl = entityModel.DefaultUserProfileCoverUrl,
                OtherMediaSaveLocation = entityModel.OtherMediaSaveLocation,
                PictureSaveLocation = entityModel.PictureSaveLocation,
                ThumbnailPictureSize = entityModel.ThumbnailPictureSize,
                MaximumFileUploadSizeForVideos = entityModel.MaximumFileUploadSizeForVideos,
                MaximumFileUploadSizeForDocuments = entityModel.MaximumFileUploadSizeForDocuments,
                MaximumFileUploadSizeForImages = entityModel.MaximumFileUploadSizeForImages
            };

            _settingService.Save(mediaSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { MediaSettings = mediaSettings.ToModel() });

        }

        [Route("post/user")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(UserSettingsModel entityModel)
        {
            var userSettings = new UserSettings()
            {
                AreUserNamesEnabled = entityModel.AreUserNamesEnabled,
                UserRegistrationDefaultMode = entityModel.UserRegistrationDefaultMode
            };
            _settingService.Save(userSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { UserSettings = userSettings.ToModel() });
        }

        [Route("post/security")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(SecuritySettingsModel entityModel)
        {
            var securitySettings = new SecuritySettings()
            {
                DefaultPasswordStorageFormat = entityModel.DefaultPasswordStorageFormat
            };
            _settingService.Save(securitySettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { SecuritySettings = securitySettings.ToModel() });
        }

        [Route("post/battle")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(BattleSettingsModel entityModel)
        {
            var battleSettings = new BattleSettings()
            {
                BattleHostPictureBattleSponsorshipPercentage = entityModel.BattleHostPictureBattleSponsorshipPercentage,
                BattleHostVideoBattleSponsorshipPercentage = entityModel.BattleHostVideoBattleSponsorshipPercentage,
                DefaultVideosFeaturedImageUrl = entityModel.DefaultVideosFeaturedImageUrl,
                DefaultVotingChargeForPaidVoting = entityModel.DefaultVotingChargeForPaidVoting,
                SiteOwnerPictureBattleSponsorshipPercentage = entityModel.SiteOwnerPictureBattleSponsorshipPercentage,
                SiteOwnerVideoBattleSponsorshipPercentage = entityModel.SiteOwnerVideoBattleSponsorshipPercentage
            };
            _settingService.Save(battleSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { BattleSettings = battleSettings.ToModel() });
        }

        [Route("post/datetime")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(DateTimeSettingsModel entityModel)
        {
            var dateTimeSettings = new DateTimeSettings()
            {
                DefaultTimeZoneId = entityModel.DefaultTimeZoneId
            };
            _settingService.Save(dateTimeSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { DateTimeSettings = dateTimeSettings.ToModel() });
        }

        [Route("post/payment")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(PaymentSettingsModel entityModel)
        {
            var paymentSettings = new PaymentSettings()
            {
                CreditExchangeRate = entityModel.CreditExchangeRate,
                IsPromotionalCreditUsageLimitPercentage = entityModel.IsPromotionalCreditUsageLimitPercentage,
                MacroPaymentsFixedPaymentProcessingFee = entityModel.MacroPaymentsFixedPaymentProcessingFee,
                MacroPaymentsPaymentProcessingPercentage = entityModel.MacroPaymentsPaymentProcessingPercentage,
                MicroPaymentsFixedPaymentProcessingFee = entityModel.MicroPaymentsFixedPaymentProcessingFee,
                MicroPaymentsPaymentProcessingPercentage = entityModel.MicroPaymentsPaymentProcessingPercentage,
                PaymentMethodSelectionType = entityModel.PaymentMethodSelectionType,
                PromotionalCreditUsageLimitPerTransaction = entityModel.PromotionalCreditUsageLimitPerTransaction
            };
            _settingService.Save(paymentSettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new {PaymentSettings = paymentSettings.ToModel()});
        }

        [Route("post/thirdparty")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(ThirdPartySettingsModel entityModel)
        {
            var thirdPartySettings = new ThirdPartySettings()
            {
                EchonestApiKey = entityModel.EchonestApiKey,
                SevenDigitalOAuthConsumerKey = entityModel.SevenDigitalOAuthConsumerKey,
                SevenDigitalOAuthConsumerSecret = entityModel.SevenDigitalOAuthConsumerSecret,
                SevenDigitalPartnerId = entityModel.SevenDigitalPartnerId
            };
            _settingService.Save(thirdPartySettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { ThirdPartySettings = thirdPartySettings.ToModel() });
        }

        [Route("post/url")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(UrlSettingsModel entityModel)
        {
            var thirdPartySettings = new UrlSettings() {
               ActivationPageUrl = entityModel.ActivationPageUrl
            };
            _settingService.Save(thirdPartySettings);
            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess(new { ThirdPartySettings = thirdPartySettings.ToModel() });
        }

        [Route("post")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(SettingEntityModel entityModel)
        {
            var setting = entityModel.Id == 0 ? new Setting() : _settingService.Get(entityModel.Id);
            if (setting == null)
                return NotFound();
           

            setting.GroupName = entityModel.GroupName;
            setting.Key = entityModel.Key;
            setting.Value = entityModel.Value;
            if (entityModel.Id == 0)
                _settingService.Insert(setting);
            else
                _settingService.Update(setting);

            VerboseReporter.ReportSuccess("Settings saved successfully", "post_setting");
            return RespondSuccess();
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        [AdminAuthorize]
        public IHttpActionResult Delete(int id)
        {
            var setting = _settingService.Get(id);
            if (setting == null)
            {
                VerboseReporter.ReportError("Setting not found", "delete_setting");
                return RespondFailure();
            }
            _settingService.Delete(setting);
            VerboseReporter.ReportSuccess("Setting deleted successfully", "delete_setting");
            return RespondSuccess();
        }
    }
}