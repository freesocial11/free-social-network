using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorTransactionModel : RootModel
    {
        public int OrderId { get; set; }

        public string TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }

        public string TransactionAmountFormatted { get; set; }
    }
}
