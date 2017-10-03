using System;
using mobSocial.Core;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class UserLike : BaseEntity, IPerApplicationEntity
    {
        public int UserId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int ApplicationId { get; set; }
    }

    public class UserLikeMap: BaseEntityConfiguration<UserLike> { }
}