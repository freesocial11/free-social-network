using System;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class UserCommentPublicModel : RootModel
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string CommentText { get; set; }

        public string AdditionalData { get; set; }

        public bool CanDelete { get; set; }

        public int LikeCount { get; set; }

        public int LikeStatus { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsSpam { get; set; }

        public string UserName { get; set; }

        public string UserProfileUrl { get; set; }

        public string UserProfileImageUrl { get; set; }

    }
}
