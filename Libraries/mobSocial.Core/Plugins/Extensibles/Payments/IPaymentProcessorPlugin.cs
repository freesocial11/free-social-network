using System.Collections.Generic;

namespace mobSocial.Core.Plugins.Extensibles.Payments
{
    public interface IPaymentProcessorPlugin : IPlugin
    {
        ITransactionProcessResult Process(ITransactionProcessRequest request, bool authorizeOnly = false);

        ITransactionPostProcessResult PostProcess(ITransactionPostProcessRequest request);

        ITransactionCaptureResult Capture(ITransactionCaptureRequest request);

        ITransactionRefundResult Refund(ITransactionRefundRequest request);

        ITransactionVoidResult Void(ITransactionVoidRequest request);

        /// <summary>
        /// Checks if the provided parameters are valid to be used by payment method
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        bool AreParametersValid(Dictionary<string, object> parameters);

        bool VoidSupported { get; }

        bool RefundSupported { get; }

        bool CaptureSupported { get; }

        /// <summary>
        /// Which payment methods does the current plugin support
        /// </summary>
        PaymentMethodType[] SupportedMethodTypes { get; }
    }
}