using System;
using mobSocial.Core;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class Comment : BaseEntity, IPerApplicationEntity
    {
        public int UserId { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string CommentText { get; set; }

        public string AdditionalData { get; set; }

        public DateTime DateCreated { get; set; }

        public string InlineTags { get; set; }

        public int ApplicationId { get; set; }
    }

    public class CommentMap: BaseEntityConfiguration<Comment> { }
}
