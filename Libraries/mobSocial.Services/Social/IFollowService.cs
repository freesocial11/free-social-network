using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface IFollowService: IBaseEntityService<UserFollow>
    {
        UserFollow GetCustomerFollow<T>(int customerId, int entityId);

        IList<UserFollow> GetCustomerFollows<T>(int customerId);

        void Insert<T>(int customerId, int entityId);

        void Delete<T>(int customerId, int entityId);

        int GetFollowerCount<T>(int entityId);

        IList<UserFollow> GetFollowers<T>(int entityId);
    }
}