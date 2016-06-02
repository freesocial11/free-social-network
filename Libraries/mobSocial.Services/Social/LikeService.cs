using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Social;

namespace mobSocial.Services.Social
{
    public class LikeService : MobSocialEntityService<UserLike>, ILikeService
    {
        public LikeService(IDataRepository<UserLike> repository) : base(repository)
        {
        }
        public UserLike GetCustomerLike<T>(int customerId, int entityId)
        {
            return
                Repository.Get(
                    x => x.EntityId == entityId && x.UserId == customerId && x.EntityName == typeof(T).Name)
                    .FirstOrDefault();
        }

        public IList<UserLike> GetCustomerLikes<T>(int customerId)
        {
            return Repository.Get(x => x.UserId == customerId && x.EntityName == typeof(T).Name).ToList();
        }

       
        public void Insert<T>(int customerId, int entityId)
        {
            //insert only if required
            if (
                !Repository.Get(
                    x =>
                        x.EntityId == entityId && x.UserId == customerId &&
                        x.EntityName == typeof (T).Name).Any())
            {
                var customerLike = new UserLike() {
                    UserId = customerId,
                    EntityId = entityId,
                    EntityName = typeof(T).Name,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                Repository.Insert(customerLike);
            }
            
        }

        public void Delete<T>(int customerId, int entityId)
        {
            var customerLike = GetCustomerLike<T>(customerId, entityId);
            if(customerLike != null)
                Repository.Delete(customerLike);
        }

        public int GetLikeCount<T>(int entityId)
        {
            return GetEntityLikes<T>(entityId).Count;
        }

        public IList<UserLike> GetEntityLikes<T>(int entityId)
        {
            return
                Repository.Get(x => x.EntityId == entityId && x.EntityName == typeof(T).Name).ToList();
        }
    }
}