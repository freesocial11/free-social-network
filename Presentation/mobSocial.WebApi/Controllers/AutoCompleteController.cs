using System.Dynamic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Skills;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
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
        private readonly ISkillService _skillService;

        #endregion

        #region ctor
        public AutoCompleteController(IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, ISkillService skillService)
        {
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _skillService = skillService;
        }

        #endregion

        [Route("{csvTypes}/get")]
        public IHttpActionResult Get(string csvTypes, [FromUri]AutoCompleteRequestModel requestModel)
        {
            if (requestModel.Count > 30)
                requestModel.Count = 30;

            var types = csvTypes.ToLowerInvariant().Split(',');//comma separated values (csv's)
            dynamic model = new ExpandoObject();
            string[] searchRoles = null;
            if (ApplicationContext.Current.CurrentUser.IsAdministrator())
            {
                searchRoles = new[] {SystemRoleNames.Registered, SystemRoleNames.Administrator};
            }
            else
            {
                searchRoles = new[] { SystemRoleNames.Registered };
            }
            //are users requested?
            if (types.Any(x => x == "users"))
            {
                var users = _userService.SearchUsers(requestModel.Search, false, searchRoles, 1, requestModel.Count);
                model.Users = users.Select(x => x.ToModel(_mediaService, _mediaSettings));
            }

            //are users requested?
            if (types.Any(x => x == "skills"))
            {
                var skills = _skillService.SearchSkills(requestModel.Search, 1, requestModel.Count);
                model.Skills = skills.Select(x => x.ToModel());
            }

            var response = RespondSuccess(new { AutoComplete = model });
            return response;
        }
    }
}