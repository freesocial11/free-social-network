using System.Web.Http;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("users")]
    public class UserController: RootApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("get")]
        [Authorize]
        public IHttpActionResult Get()
        {
            var user = ApplicationContext.Current.CurrentUser;
            return Json(new {user.Email});
        }
    }
}