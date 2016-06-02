using System.Collections.Generic;
using mobSocial.Core.Plugins.Extensibles.Payments;

namespace mobSocial.Data.Entity.Payments
{
    /// <summary>
    /// A single request class to implement all types of transaction requests
    /// </summary>
    public class TransactionRequest : 
        ITransactionProcessRequest, 
        ITransactionCaptureRequest, 
        ITransactionPostProcessRequest,
        ITransactionRefundRequest,
        ITransactionVoidRequest
    {
        public int UserId { get; set; }

        public int AddressId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentProcessorSystemName { get; set; }

        public string CurrencyIsoCode { get; set; }

        public string TransactionUniqueId { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public bool IsPartialRefund { get; set; }
    }
}