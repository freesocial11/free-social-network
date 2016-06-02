using mobSocial.Core.Config;

namespace mobSocial.Data.Entity.Settings
{
    public class BattleSettings: ISettingGroup
    {
        public decimal DefaultVotingChargeForPaidVoting { get; set; }

        public string DefaultVideosFeaturedImageUrl { get; set; }

        public decimal SiteOwnerVideoBattleSponsorshipPercentage { get; set; }

        public decimal BattleHostVideoBattleSponsorshipPercentage { get; set; }

        public decimal SiteOwnerPictureBattleSponsorshipPercentage { get; set; }

        public decimal BattleHostPictureBattleSponsorshipPercentage { get; set; }
    }
}