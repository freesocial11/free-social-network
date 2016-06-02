using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorTransactionRequestModel : RootModel
    {
        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public int CustomerId { get; set; }

    }
}