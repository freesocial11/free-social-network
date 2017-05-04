using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.AutoComplete
{
    public class AutoCompleteRequestModel : RootModel
    {
        public string Search { get; set; }

        public int Count { get; set; }

        public bool ExcludeLoggedInUser { get; set; }
    }
}