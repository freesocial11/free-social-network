using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Payments;

namespace mobSocial.Services.Payments
{
    public interface IUserPaymentMethodService : IBaseEntityService<UserPaymentMethod>
    {
        IList<UserPaymentMethod> GetCustomerPaymentMethods(int customerId, bool verifiedOnly = false);

        bool DoesCardNumberExist(string cardNumber);
    }
}
