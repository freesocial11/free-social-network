using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class UserGetModel : RootModel
    {
        public string SearchText { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

    }
}