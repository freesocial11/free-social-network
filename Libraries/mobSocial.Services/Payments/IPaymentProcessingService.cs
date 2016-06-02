using System.Collections.Generic;
using mobSocial.Core.Plugins.Extensibles.Payments;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Payments
{
    public interface IPaymentProcessingService
    {
        ITransactionProcessResult ProcessPayment(User user, UserPaymentMethod paymentMethod, ITransactionProcessRequest processRequest, bool authorizeOnly = false);

        ITransactionCaptureResult CapturePayment(PaymentTransaction transaction);

        ITransactionVoidResult VoidPayment(PaymentTransaction transaction);

        ITransactionRefundResult RefundPayment(PaymentTransaction transaction);

        IPaymentProcessorPlugin GetPaymentProcessorPlugin(decimal amount, PaymentMethodType methodType);

        decimal GetNetAmountAfterPaymentProcessing(decimal amount);

        IList<IPaymentProcessorPlugin> GetSupportedPaymentProcessorPlugins(params PaymentMethodType[] methodTypes);
    }
}