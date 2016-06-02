using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class UserCommentRequestModel: RootModel
    {
        [Required]
        public string EntityName { get; set; }

        [Required]
        public int EntityId { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }
    }
}
