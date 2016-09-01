using System.Linq;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface IFollowService: IBaseEntityService<UserFollow>
    {
        UserFollow GetCustomerFollow<T>(int customerId, int entityId);

        IQueryable<UserFollow> GetFollowing<T>(int customerId, int page = 1, int count = 15);

        IQueryable<UserFollow> GetFollowing(int customerId, int page = 1, int count = 15);

        void Insert<T>(int customerId, int entityId);

        void Delete<T>(int customerId, int entityId);

        int GetFollowerCount<T>(int entityId);

        IQueryable<UserFollow> GetFollowers<T>(int entityId, int page = 1, int count = 15);
    }
}