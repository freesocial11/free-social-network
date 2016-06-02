using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public interface ILikeService: IBaseEntityService<UserLike>
    {
        UserLike GetCustomerLike<T>(int customerId, int entityId);

        IList<UserLike> GetCustomerLikes<T>(int customerId);

        void Insert<T>(int customerId, int entityId);

        void Delete<T>(int customerId, int entityId);

        int GetLikeCount<T>(int entityId);

        IList<UserLike> GetEntityLikes<T>(int entityId);
    }
}