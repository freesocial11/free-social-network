using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class UpdateSponsorModel: RootModel
    {
        public int SponsorCustomerId { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public SponsorshipStatus SponsorshipStatus { get; set; }

        public string TargetUrl { get; set; }

        public string DisplayName { get; set; }

        public int PictureId { get; set; }
    }
}