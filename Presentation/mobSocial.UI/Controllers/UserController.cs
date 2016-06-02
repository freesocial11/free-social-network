using System.Web.Mvc;

namespace mobSocial.UI.Controllers
{
    public class UserController : RootController
    {

        [Route("~/login", Name = "LoginPage")]
        public ActionResult Login()
        {
            return View("Login");
        }

        [Route("~/register", Name = "RegistrationPage")]
        public ActionResult Register()
        {
            return View("Register");
        }
    }
}