using mobSocial.Core.Data;
using mobSocial.Core.Plugins.Extensibles.Payments;

namespace mobSocial.Data.Entity.Payments
{
    public class UserPaymentMethod : BaseEntity
    {
        public int UserId { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public string CardNumber { get; set; }

        public string CardNumberMasked { get; set; }

        public string NameOnCard { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }

        public string CardIssuerType { get; set; }

        public bool IsVerified { get; set; }

    }
}
