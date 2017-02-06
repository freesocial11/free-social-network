using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Skills;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Skills;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.Skills;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("skills")]
    public class SkillController : RootApiController
    {
        #region fields
        private readonly ISkillService _skillService;
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IUserSkillService _userSkillService;
        private readonly GeneralSettings _generalSettings;
        #endregion

        #region ctor
        public SkillController(ISkillService skillService, IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, IUserSkillService userSkillService, GeneralSettings generalSettings)
        {
            _skillService = skillService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _userSkillService = userSkillService;
            _generalSettings = generalSettings;
        }
        #endregion

        #region actions
        [HttpGet]
        [Route("users/{userId:int}/get")]
        public IHttpActionResult GetUserSkills(int userId)
        {
            //check if the user exists or not
            var customer = _userService.Get(userId);
            if (customer == null)
                return NotFound();

            var userSkills = _skillService.GetUserSkills(userId).OrderBy(x => x.DisplayOrder);
            var model = userSkills.Select(x => x.ToModel(_mediaService, _mediaSettings, _generalSettings, true));

            return RespondSuccess(new { Skills = model });
        }

        [HttpGet]
        [Route("get/all")]
        public IHttpActionResult GetSystemSkills(int page = 1, int count = 15)
        {
            int total;
            var skills = _skillService.GetAllSkills(out total, string.Empty, page, count);
            var model = skills.Select(x => x.ToModel()).ToList();
            return RespondSuccess(new { Skills = model, Total = total });
        }

        [HttpGet]
        [Authorize]
        [Route("get/{id:int}")]
        public IHttpActionResult GetSkill(int id)
        {
            //get the skill first
            var skill = _skillService.Get(id);
            if (skill == null)
                return NotFound();
            var model = skill.ToModel();
            return RespondSuccess(new { Skill = model });
        }

        [HttpPost]
        [Authorize]
        [Route("post")]
        public IHttpActionResult Post(UserSkillEntityModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var currentUser = ApplicationContext.Current.CurrentUser;
            //if it's admin, we can safely change the customer id otherwise we'll save skill as logged in user 
            var isAdmin = currentUser.IsAdministrator();
            if (!isAdmin && model.UserId > 0)
                model.UserId = currentUser.Id;

            if (model.SystemSkill && isAdmin)
                model.UserId = 0;
            else
                model.UserId = currentUser.Id;

            if (model.MediaId > 0)
            {
                //so there is a media, let's see if user owns this media
                var media = _mediaService.Get(model.MediaId);
                if (media == null || media.UserId != model.UserId)
                    return Unauthorized();
            }

            //get skill, 1.) by id 2.) by name 3.) create new otherwise
            var skill = _skillService.Get(model.Id) ??
                        (_skillService.FirstOrDefault(x => x.SkillName == model.SkillName) ?? new Skill()
                         {
                             DisplayOrder = model.DisplayOrder,
                             UserId = currentUser.Id,
                             SkillName = model.SkillName
                         });

            //should we add this?
            if (skill.Id == 0)
            {
                _skillService.Insert(skill);
            }
            else
            {
                if (model.SystemSkill && isAdmin)
                {
                    skill.SkillName = model.SkillName;
                    _skillService.Update(skill);
                }
            }

            //if user id is not 0, attach this skill with user
            if (model.UserId != 0)
            {
                var userSkill = model.UserSkillId > 0 ? _userSkillService.Get(model.UserSkillId) : new UserSkill()
                {
                    UserId = model.UserId,
                    SkillId = skill.Id,
                    Description = model.Description,
                    DisplayOrder = model.DisplayOrder,
                    ExternalUrl = model.ExternalUrl
                };

                if (userSkill.Id == 0)
                    _userSkillService.Insert(userSkill);
                else
                    _userSkillService.Update(userSkill);

                //attach media if it exists
                _mediaService.ClearEntityMedia(userSkill);
                if (model.MediaId > 0)
                    _mediaService.AttachMediaToEntity<UserSkill>(userSkill.Id, model.MediaId);
                return RespondSuccess(new
                {
                    Skill = userSkill.ToModel(_mediaService, _mediaSettings, _generalSettings)
                });
            }
            return RespondSuccess(new
            {
                Skill = skill.ToModel()
            });
        }

        [HttpDelete]
        [Authorize]
        [Route("delete/{skillId:int}")]
        public IHttpActionResult Delete(int skillId)
        {
           
            var currentUser = ApplicationContext.Current.CurrentUser;
            //current user must be admin to delete this skill
            if(!currentUser.IsAdministrator())
                return Unauthorized();

            var skill = _skillService.Get(skillId);
            if (skill == null)
                return NotFound();

            _userSkillService.Delete(x => x.SkillId == skillId);
            //so we can safely delete this
            _skillService.Delete(skill);
            return RespondSuccess();
        }

        [HttpDelete]
        [Authorize]
        [Route("users/delete/{userSkillId:int}")]
        public IHttpActionResult DeleteUserSkill(int userSkillId)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;

            //first query user skill
            var userSkill = _userSkillService.Get(userSkillId);

            //the current user should be either admin or himself to delete the skill
            if (userSkill.UserId != currentUser.Id && !currentUser.IsAdministrator())
                return Unauthorized();

            //detach media
            _mediaService.ClearEntityMedia(userSkill);

            _userSkillService.Delete(userSkill);

            return RespondSuccess();
        }

        #endregion
    }
}