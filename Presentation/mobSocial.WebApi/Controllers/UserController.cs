using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
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
        [HttpGet]
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

        [Route("get/{id:int}")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Get(int id)
        {
            //we need to make sure that logged in user has capability to view user
            if (!(ApplicationContext.Current.CurrentUser.IsAdministrator() || ApplicationContext.Current.CurrentUser.Id != id))
                return NotFound();

            //first get the user
            var user = await _userService.GetAsync(id);
            if (user == null)
                return NotFound();

            return RespondSuccess(new
            {
                User = user.ToEntityModel()
            });
        }

        [Route("post")]
        [HttpPost]
        [AdminAuthorize]
        public IHttpActionResult Post(UserEntityModel entityModel)
        {
            User user;
            user = entityModel.Id == 0 ? new User() : _userService.Get(entityModel.Id);

            if (user == null)
                return NotFound();

            user.FirstName = entityModel.FirstName;
            user.LastName = entityModel.LastName;
            user.Email = entityModel.Email;
            user.Remarks = entityModel.Remarks;
            user.Active = entityModel.Active;
            user.DateUpdated = DateTime.UtcNow;
            user.Name = string.Concat(user.FirstName, user.LastName);
            if (entityModel.Id == 0)
            {
                user.Guid = Guid.NewGuid();
                user.DateCreated = DateTime.UtcNow;
                user.IsSystemAccount = false;
                user.LastLoginDate = DateTime.UtcNow;
                user.Password = entityModel.Password;
                user.PasswordFormat = PasswordFormat.Md5Hashed;
                _userService.Insert(user);
            }
            else
            {
                _userService.Update(user);
            }
            return RespondSuccess(new
            {
                User = user.ToEntityModel()
            });
        }
    }
}