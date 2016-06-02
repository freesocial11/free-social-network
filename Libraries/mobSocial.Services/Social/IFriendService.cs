using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface IFriendService : IBaseEntityService<Friend>
    {
        Friend GetCustomerFriendship(int customer1Id, int customer2Id);

        Friend GetCustomerFriend(int fromCustomerId, int toCustomerId);

        IList<Friend> GetCustomerFriendRequests(int customerId);

        IList<Friend> GetCustomerFriends(int customerId, int count = 0, bool random = false);

        IList<Friend> GetAllCustomerFriends(int customerId);
    }
}