using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Applications
{
    public class ApplicationLoginModel : RootEntityModel
    {
        public string Name { get; set; }

        public string ApplicationUrl { get; set; }
    }
}