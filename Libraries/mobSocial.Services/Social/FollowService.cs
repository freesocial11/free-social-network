using System;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public class FollowService : MobSocialEntityService<UserFollow>, IFollowService
    {
        public FollowService(IDataRepository<UserFollow> repository) : base(repository)
        {
        }
        public UserFollow GetCustomerFollow<T>(int customerId, int entityId)
        {
            return
                Repository.Get(
                    x =>
                        x.FollowingEntityId == entityId && x.UserId == customerId &&
                        x.FollowingEntityName == typeof(T).Name).FirstOrDefault();
        }

        public IQueryable<UserFollow> GetFollowing<T>(int customerId, int page = 1, int count = 15)
        {
            return
                Get(x => x.UserId == customerId && x.FollowingEntityName == typeof(T).Name, null, true, page, count);
        }

        public IQueryable<UserFollow> GetFollowing(int customerId, int page = 1, int count = 15)
        {
            return
                Get(x => x.UserId == customerId, null, true, page, count);
        }

        public void Insert<T>(int customerId, int entityId)
        {
            //insert only if required
            if (!Repository.Get(
                    x =>
                        x.FollowingEntityId == entityId && x.UserId == customerId &&
                        x.FollowingEntityName == typeof (T).Name).Any())
            {
                var customerFollow = new UserFollow() {
                    UserId = customerId,
                    FollowingEntityId = entityId,
                    FollowingEntityName = typeof(T).Name,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                Repository.Insert(customerFollow);
            }
            
        }

        public void Delete<T>(int customerId, int entityId)
        {
            var customerFollow = GetCustomerFollow<T>(customerId, entityId);
            if(customerFollow != null)
                Repository.Delete(customerFollow);
        }

        public int GetFollowerCount<T>(int entityId)
        {
            return GetFollowers<T>(entityId).Count();
        }

        public IQueryable<UserFollow> GetFollowers<T>(int entityId, int page = 1, int count = 15)
        {
            return
               Get(x => x.FollowingEntityId == entityId && x.FollowingEntityName == typeof (T).Name, null, true, page, count);
        }
    }
}