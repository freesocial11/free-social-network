using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Social
{
    public class FriendSearchModel : RootModel
    {
        public string SearchTerm { get; set; }

        public bool ExcludeLoggedInUser { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }
    }
}