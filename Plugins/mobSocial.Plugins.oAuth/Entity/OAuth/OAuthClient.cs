using mobSocial.Core.Data;
using mobSocial.Data.Entity;
using mobSocial.Plugins.OAuth.Enums;

namespace mobSocial.Plugins.OAuth.Entity.OAuth
{
    public class OAuthClient : BaseEntity
    {
        public string Guid { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public string RedirectUri { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public string AllowedOrigin { get; set; }

        public int RequestLimitPerHour { get; set; }
    }

    public class OAuthClientMap : BaseEntityConfiguration<OAuthClient>
    {
        
    }
}
