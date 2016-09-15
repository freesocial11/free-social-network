using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class UserActivationModel : RootModel
    {
        public string ActivationCode { get; set; }

        public string Email { get; set; }
    }
}