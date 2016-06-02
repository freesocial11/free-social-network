namespace mobSocial.Core.Plugins.Extensibles.Payments
{
    public interface ITransactionProcessRequest : ITransactionRequestBase
    {
        string TransactionUniqueId { get; set; }
    }
}