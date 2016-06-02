using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoBattlePrizePublicModel : RootModel
    {
        public string WinningPosition { get; set; }

        public string PrizeType { get; set; }

        public string FormattedPrize { get; set; }

        public string SponsorName { get; set; }

        public string SponsorCustomerUrl { get; set; }
    }
}