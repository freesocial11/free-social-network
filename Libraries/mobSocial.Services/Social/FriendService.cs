using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Enum;

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
                Get(x =>
                    (x.FromCustomerId == customer1Id && x.ToCustomerId == customer2Id) ||
                    (x.ToCustomerId == customer1Id && x.FromCustomerId == customer2Id)).FirstOrDefault();
        }


        public Friend GetCustomerFriend(int fromCustomerId, int toCustomerId)
        {
            return
                Get(x => (x.FromCustomerId == fromCustomerId && x.ToCustomerId == toCustomerId)).FirstOrDefault();
        }

        public FriendStatus GetFriendStatus(int currentUserId, int friendId)
        {
            if(currentUserId == friendId)
                return FriendStatus.Self;

            var friend = GetCustomerFriendship(currentUserId, friendId);
            if(friend == null)
                return FriendStatus.None;

            if(friend.Confirmed)
                return FriendStatus.Friends;

            return friend.FromCustomerId == currentUserId ? FriendStatus.FriendRequestSent : FriendStatus.NeedsConfirmed;
        }


        public IList<Friend> GetFriendRequests(int customerId)
        {
            return Get(x => !x.Confirmed && x.ToCustomerId == customerId).ToList();
        }

        public IQueryable<Friend> GetFriends(int customerId, int page = 1, int count = 15, bool random = false)
        {
            var friends =
                Get(x => (x.FromCustomerId == customerId || x.ToCustomerId == customerId) && x.Confirmed, null, true,
                    page, count);

            if (random)
                friends = friends.OrderBy(x => Guid.NewGuid());

            return friends;
        }

        public IList<Friend> GetAllCustomerFriends(int customerId)
        {
            return Get(x => x.FromCustomerId == customerId || x.ToCustomerId == customerId).ToList();
        }
    }
}