using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("roles")]
    public class RoleController : RootApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Route("get")]
        [HttpGet]
        [AdminAuthorize]
        public async Task<IHttpActionResult> Get()
        {
            var roles = await _roleService.GetAsync();
            var roleModels = roles.ToList().Select(x => x.ToModel());
            return RespondSuccess(new
            {
                Roles = roleModels
            });
        }
    }
}