using mobSocial.Core.Plugins.Extensibles.Payments;

namespace mobSocial.WebApi.Models.Payment
{
    public class UserPaymentRequestModel
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethodType PaymentMethod { get; set; }

        public string CardNumber { get; set; }

        public string NameOnCard { get; set; }

        public string SecurityCode { get; set; }

        public string ExpireMonth { get; set; }

        public string ExpireYear { get; set; }

        public string CardIssuerType { get; set; }

        public bool IsVerified { get; set; }
    }
}
