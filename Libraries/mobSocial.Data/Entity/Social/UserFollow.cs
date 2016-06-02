using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class UserFollow : BaseEntity
    {
        public int UserId { get; set; }

        public int FollowingEntityId { get; set; }

        public string FollowingEntityName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }

    public class UserFollowMap: BaseEntityConfiguration<UserFollow> { }
}