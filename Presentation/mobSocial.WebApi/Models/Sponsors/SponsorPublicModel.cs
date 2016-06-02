using System.Collections.Generic;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Battles;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorPublicModel : RootModel
    {
       
        public string SponsorName { get; set; }

        public string SeName { get; set; }

        public int CustomerId { get; set; }

        public string SponsorProfileImageUrl { get; set; }

        public SponsorshipStatus SponsorshipStatus { get; set; }

        public string SponsorshipStatusName { get; set; }

        public decimal SponsorshipAmount { get; set; }

        public string SponsorshipAmountFormatted { get; set; }

        public SponsorDataModel SponsorData { get; set; }

        public SponsorshipType SponsorshipType { get; set; }

        public IList<VideoBattlePrizeModel> SponsoredProductPrizes { get; set; }

    }
}