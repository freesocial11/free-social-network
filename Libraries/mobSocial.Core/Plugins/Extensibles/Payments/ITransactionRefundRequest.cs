namespace mobSocial.Core.Plugins.Extensibles.Payments
{
    public interface ITransactionRefundRequest : ITransactionRequestBase
    {
        bool IsPartialRefund { get; set; }
    }
}