using System.Collections.Generic;
using mobSocial.Core.Plugins.Extensibles.Payments;

namespace mobSocial.Data.Entity.Payments
{
    public class TransactionResult : ITransactionProcessResult, ITransactionCaptureResult, ITransactionPostProcessResult, ITransactionRefundResult, ITransactionVoidResult
    {
        public bool Success { get; set; }

        public Dictionary<string, object> ResponseParameters { get; set; }
    }
}