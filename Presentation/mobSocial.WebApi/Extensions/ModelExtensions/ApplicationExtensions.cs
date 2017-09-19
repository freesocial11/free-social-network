using mobSocial.Data.Entity.OAuth;
using mobSocial.WebApi.Models.Applications;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class ApplicationExtensions
    {
        public static ApplicationModel ToModel(this OAuthApplication application)
        {
            var model = new ApplicationModel()
            {
                ClientId = application.Guid,
                Active = application.Active,
                Id = application.Id,
                ApplicationType = application.ApplicationType,
                AllowedOrigin = application.AllowedOrigin,
                ApplicationUrl = application.ApplicationUrl,
                ClientSecret = application.Secret,
                Description = application.Description,
                Name = application.Name,
                PrivacyPolicyUrl = application.PrivacyPolicyUrl,
                RedirectUrl = application.RedirectUrl,
                TermsUrl = application.TermsUrl,
                RequestLimitPerHour = application.RequestLimitPerHour
            };
            return model;
        }

        public static ApplicationMiniModel ToMiniModel(this OAuthApplication application)
        {
            var model = new ApplicationMiniModel()
            {
                Id = application.Id,
                Active = application.Active,
                Name = application.Name
            };
            return model;
        }
    }
}