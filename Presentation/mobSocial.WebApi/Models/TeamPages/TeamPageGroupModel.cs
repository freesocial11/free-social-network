using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPageGroupModel : RootEntityModel
    {
        public int TeamPageId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string PayPalDonateUrl { get; set; }

        public virtual int DisplayOrder { get; set; }

        public virtual bool IsDefault { get; set; }

    }
}