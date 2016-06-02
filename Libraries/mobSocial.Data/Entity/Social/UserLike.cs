using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class UserLike : BaseEntity
    {
        public int UserId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }

    public class UserLikeMap: BaseEntityConfiguration<UserLike> { }
}