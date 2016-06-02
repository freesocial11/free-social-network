using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class UserCommentModel : RootEntityModel
    {
        [Required]
        public int EntityId { get; set; }

        [Required]
        public string EntityName { get; set; }

        [Required]
        public string CommentText { get; set; }

        public string AdditionalData { get; set; }
    }
}
