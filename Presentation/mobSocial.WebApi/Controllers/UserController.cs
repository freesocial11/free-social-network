using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Security.Attributes;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Users;

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
        [AdminAuthorize]
        public async Task<IHttpActionResult> Get([FromUri] UserGetModel requestModel)
        {
            //do we have valid values?
            if (requestModel.Page <= 0)
                requestModel.Page = 1;

            //do the needful
            var users = await _userService.GetAsync(x =>
                (!string.IsNullOrEmpty(requestModel.SearchText) && 
                (x.FirstName.StartsWith(requestModel.SearchText) ||
                x.LastName.StartsWith(requestModel.SearchText) ||
                x.Email == requestModel.SearchText)) || true, null, true, requestModel.Page, requestModel.Count);

            var model = users.ToList().Select(user => user.ToModel());
            return RespondSuccess(new
            {
                Users = model
            });
        }
    }
}