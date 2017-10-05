using System.Web.Http;
using mobSocial.WebApi.Configuration;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    public class DefaultController : RootApiController
    {
        [HttpGet]
        public IHttpActionResult Index()
        {
            return RespondSuccess(new {Version = Globals.Version});
        }
    }
}