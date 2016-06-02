using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class Comment : BaseEntity
    {
        public int UserId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string CommentText { get; set; }

        public string AdditionalData { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class CommentMap: BaseEntityConfiguration<Comment> { }
}
