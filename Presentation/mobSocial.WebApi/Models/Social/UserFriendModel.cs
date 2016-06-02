using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class UserFriendModel : RootModel
    {


        public string CustomerDisplayName { get; set; }

        public string ProfileUrl { get; set; }

        public string ProfileThumbnailUrl { get; set; }
    }
}