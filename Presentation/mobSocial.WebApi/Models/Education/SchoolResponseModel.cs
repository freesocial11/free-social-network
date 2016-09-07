using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Education
{
    public class SchoolResponseModel : RootModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string LogoUrl { get; set; }
    }
}