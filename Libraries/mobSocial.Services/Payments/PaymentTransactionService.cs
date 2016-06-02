using mobSocial.Core.Data;
using mobSocial.Data.Entity.Payments;

namespace mobSocial.Services.Payments
{
    public class PaymentTransactionService : MobSocialEntityService<PaymentTransaction>, IPaymentTransactionService
    {
        public PaymentTransactionService(IDataRepository<PaymentTransaction> dataRepository) : base(dataRepository)
        {
        }
    }
}