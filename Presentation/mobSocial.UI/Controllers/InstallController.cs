using System.Web.Mvc;

namespace mobSocial.UI.Controllers
{
    public class InstallController : RootController
    {
        [Route("~/install")]
        public ActionResult Install()
        {
            return View();
        }
    }
}