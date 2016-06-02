using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Payments;

namespace mobSocial.Services.Payments
{
    public class UserPaymentMethodService : MobSocialEntityService<UserPaymentMethod>, IUserPaymentMethodService
    {
       
        public UserPaymentMethodService(IDataRepository<UserPaymentMethod> repository) : base(repository)
        {
        }
        public IList<UserPaymentMethod> GetCustomerPaymentMethods(int customerId, bool verifiedOnly = false)
        {
            var query = Repository.Get(x => x.UserId == customerId);
            if (verifiedOnly)
            {
                query = query.Where(x => x.IsVerified);
            }

            return query.ToList();
        }

        public bool DoesCardNumberExist(string cardNumber)
        {
            return Repository.Get(x => x.CardNumber == cardNumber).Any();
        }
    }
}