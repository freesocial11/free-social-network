using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Payment;

namespace mobSocial.WebApi.Models.Credits
{
    public class PurchaseCreditModel : RootModel
    {
        public int CustomerPaymentMethodId { get; set; }

        public int UseCreditCount { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public UserPaymentRequestModel CustomerPaymentRequest { get; set; }

        public PurchaseType PurchaseType { get; set; }
    }
}
