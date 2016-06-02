using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoBattlePrizeModel : RootEntityModel
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
    }
}
