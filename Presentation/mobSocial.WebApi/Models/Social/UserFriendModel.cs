using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class UserFriendModel : RootModel
    {


        public string DisplayName { get; set; }

        public string SeName { get; set; }

        public string ProfileThumbnailUrl { get; set; }
    }
}