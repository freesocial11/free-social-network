using System.Collections.Generic;

namespace mobSocial.Core.Plugins.Extensibles.Payments
{
    public interface ITransactionRequestBase
    {
        int UserId { get; set; }

        int AddressId { get; set; }

        decimal Amount { get; set; }

        string PaymentProcessorSystemName { get; set; }

        string CurrencyIsoCode { get; set; }

        Dictionary<string, object> Parameters { get; set; }

    }
}