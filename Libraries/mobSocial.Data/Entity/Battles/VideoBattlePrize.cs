using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattlePrize: BaseEntity
    {
        public int VideoBattleId { get; set; }

        public int WinnerPosition { get; set; }

        public BattlePrizeType PrizeType { get; set; }

        public string Description { get; set; }

        public decimal PrizePercentage { get; set; }

        public decimal PrizeAmount { get; set; }

        public int PrizeProductId { get; set; }

        public string PrizeOther { get; set; }

        public int WinnerId { get; set; }

        public bool IsSponsored { get; set; }

        public int SponsorCustomerId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

    }

    public class VideoBattlePrizeMap: BaseEntityConfiguration<VideoBattlePrize> { }
}
