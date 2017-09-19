using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.OAuth
{
    public class OAuthApplication : BaseEntity
    {
        public string Guid { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ApplicationUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public string TermsUrl { get; set; }

        public string RedirectUrl { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }

        public int RequestLimitPerHour { get; set; }
    }

    public class OAuthApplicationMap : BaseEntityConfiguration<OAuthApplication>
    {
    }
}
