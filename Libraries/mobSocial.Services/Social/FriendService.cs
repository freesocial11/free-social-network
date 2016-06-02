using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public class FriendService : MobSocialEntityService<Friend>, IFriendService
    {
        public FriendService(IDataRepository<Friend> repository)
            : base(repository)
        {
        }

        public Friend GetCustomerFriendship(int customer1Id, int customer2Id)
        {
            return
                Repository.Get(x =>
                    (x.FromCustomerId == customer1Id && x.ToCustomerId == customer2Id) ||
                    (x.ToCustomerId == customer1Id && x.FromCustomerId == customer2Id)).FirstOrDefault();
        }


        public Friend GetCustomerFriend(int fromCustomerId, int toCustomerId)
        {
            return
                Repository.Get(x => (x.FromCustomerId == fromCustomerId && x.ToCustomerId == toCustomerId)).FirstOrDefault();
        }


        public IList<Friend> GetCustomerFriendRequests(int customerId)
        {
            return Repository.Get(x => !x.Confirmed && x.ToCustomerId == customerId).ToList();
        }

        public IList<Friend> GetCustomerFriends(int customerId, int count = 0, bool random = false)
        {
            var friends =
                Repository.Get(x => (x.FromCustomerId == customerId || x.ToCustomerId == customerId) && x.Confirmed);

            if (random)
                friends = friends.OrderBy(x => Guid.NewGuid());

            if (count > 0)
                friends = friends.Take(count);
            return friends.ToList();
        }

        public IList<Friend> GetAllCustomerFriends(int customerId)
        {
            return Repository.Get(x => x.FromCustomerId == customerId || x.ToCustomerId == customerId).ToList();
        }
    }
}