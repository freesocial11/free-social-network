using System;
using mobSocial.Core;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class UserFollow : BaseEntity, IPerApplicationEntity
    {
        public int UserId { get; set; }

        public int FollowingEntityId { get; set; }

        public string FollowingEntityName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int ApplicationId { get; set; }
    }

    public class UserFollowMap: BaseEntityConfiguration<UserFollow> { }
}