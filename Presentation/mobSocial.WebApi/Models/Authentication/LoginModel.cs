using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Authentication
{
    public partial class LoginModel : RootModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Persist { get; set; }

        public string ReturnUrl { get; set; }

        public bool Redirect { get; set; }
    }
}