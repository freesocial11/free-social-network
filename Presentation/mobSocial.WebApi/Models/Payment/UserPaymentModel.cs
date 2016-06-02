using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Payment
{
    public class UserPaymentModel : RootModel
    {
        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public PurchaseType PurchaseType { get; set; }
    }
}