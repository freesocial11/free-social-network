using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorsRequestModel : RootModel
    {
        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public SponsorshipStatus SponsorshipStatus { get; set; }
    }
}