using System.Collections.Generic;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Battles;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorModel: RootEntityModel
    {
        public SponsorModel()
        {
            Prizes = new List<VideoBattlePrizeModel>();    
        }

        public decimal SponsorshipCredits { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public SponsorshipType SponsorshipType { get; set; }

        public IList<VideoBattlePrizeModel> Prizes { get; set; }
    }
}