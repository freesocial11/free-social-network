using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Education
{
    public class SchoolSearchModel : RootModel
    {
        public string SearchText { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }
    }
}