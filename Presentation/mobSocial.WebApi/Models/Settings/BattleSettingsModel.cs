using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Settings
{
    public class BattleSettingsModel: RootModel
    {
        public decimal DefaultVotingChargeForPaidVoting { get; set; }

        public string DefaultVideosFeaturedImageUrl { get; set; }

        public decimal SiteOwnerVideoBattleSponsorshipPercentage { get; set; }

        public decimal BattleHostVideoBattleSponsorshipPercentage { get; set; }

        public decimal SiteOwnerPictureBattleSponsorshipPercentage { get; set; }

        public decimal BattleHostPictureBattleSponsorshipPercentage { get; set; }
    }
}