using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.ArtistPages
{
    public class ArtistPagePaymentModel: RootEntityModel
    {
        public int ArtistPageId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaypalEmail { get; set; }
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string PayableTo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
