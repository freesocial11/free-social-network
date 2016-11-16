using System.Dynamic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.AutoComplete;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("autocomplete")]
    public class AutoCompleteController : RootApiController
    {
        #region fields
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        #endregion

        #region ctor
        public AutoCompleteController(IUserService userService, IMediaService mediaService, MediaSettings mediaSettings)
        {
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        #endregion

        [Route("{csvTypes}/get")]
        public IHttpActionResult Get(string csvTypes, [FromUri]AutoCompleteRequestModel requestModel)
        {
            if (requestModel.Count > 30)
                requestModel.Count = 30;

            var types = csvTypes.ToLowerInvariant().Split(',');//comma separated values (csv's)
            dynamic model = new ExpandoObject();
            //are users requested?
            if (types.Any(x => x == "users"))
            {
                var users = _userService.SearchUsers(requestModel.Search, false, new[] { SystemRoleNames.Registered }, 1, requestModel.Count);
                model.Users = users.Select(x => x.ToModel(_mediaService, _mediaSettings));
            }
            var response = RespondSuccess(new { AutoComplete = model });
            return response;
        }
    }
}