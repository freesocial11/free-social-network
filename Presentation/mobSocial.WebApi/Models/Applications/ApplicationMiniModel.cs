using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Applications
{
    public class ApplicationMiniModel : RootEntityModel
    {
        public string Name { get; set; }

        public bool Active { get; set; }
    }
}