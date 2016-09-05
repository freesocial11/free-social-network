using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class ChangePasswordModel : RootModel
    {
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}