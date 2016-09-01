using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Social
{
    public interface IFriendService : IBaseEntityService<Friend>
    {
        Friend GetCustomerFriendship(int customer1Id, int customer2Id);

        Friend GetCustomerFriend(int fromCustomerId, int toCustomerId);

        FriendStatus GetFriendStatus(int currentUserId, int friendId);

        IList<Friend> GetFriendRequests(int customerId);

        IQueryable<Friend> GetFriends(int customerId, int page = 1, int count = 15, bool random = false);

        IList<Friend> GetAllCustomerFriends(int customerId);
    }
}