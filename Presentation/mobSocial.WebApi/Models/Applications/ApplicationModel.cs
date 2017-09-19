using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Applications
{
    public class ApplicationModel : RootEntityModel
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ApplicationUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public string TermsUrl { get; set; }

        public string RedirectUrl { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public string AllowedOrigin { get; set; }

        public int RequestLimitPerHour { get; set; }
    }
}