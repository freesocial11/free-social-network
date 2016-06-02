using System.Security.Claims;
using System.Web.Mvc;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.Plugins.OAuth.Controllers
{
    [RoutePrefix("oauth2")]
    public class OAuthController : RootController
    {
        [Route("authorize")]
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("OAuth/AuthorizeError");
            }
            if (ApplicationContext.Current.CurrentUser == null)
            {
                //challenge the authentication
                ApplicationContext.Current.CurrentOwinContext.Authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }
            if (Request.HttpMethod == "POST")
            {
                var context = ApplicationContext.Current.CurrentOwinContext;
                var authentication = context.Authentication;
                var ticket = authentication.AuthenticateAsync("Application").Result;
                var identity = ticket != null ? ticket.Identity : null;
                identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                identity.AddClaim(new Claim("urn:oauth:scope", "testClaim"));
                authentication.SignIn(identity);
            }
            return View("OAuth/Authorize");
        }

    }
}
