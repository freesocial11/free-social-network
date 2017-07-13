using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Skills;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Permalinks;
using mobSocial.Services.Skills;
using mobSocial.Services.Social;
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
        private readonly IPermalinkService _permalinkService;
        private readonly IFollowService _followService;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;
        private readonly SkillSettings _skillSettings;
        #endregion

        #region ctor
        public SkillController(ISkillService skillService, IUserService userService, IMediaService mediaService, MediaSettings mediaSettings, IUserSkillService userSkillService, GeneralSettings generalSettings, IPermalinkService permalinkService, IFollowService followService, ILikeService likeService, ICommentService commentService, SkillSettings skillSettings)
        {
            _skillService = skillService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _userSkillService = userSkillService;
            _generalSettings = generalSettings;
            _permalinkService = permalinkService;
            _followService = followService;
            _likeService = likeService;
            _commentService = commentService;
            _skillSettings = skillSettings;
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
            return RespondSuccess(new { Skills = model, Total = total, Page = page, Count = count });
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

        [HttpGet]
        [Route("get/{slug}")]
        public IHttpActionResult GetSkill(string slug)
        {
            var permalink = _permalinkService.FirstOrDefault(x => x.Slug == slug && x.EntityName == typeof(Skill).Name);
            if (permalink == null)
                return NotFound();

            var skillId = permalink.EntityId;
            var skill = _skillService.Get(skillId);

            if (skill == null)
                return NotFound();

            var model = skill.ToSkillWithUsersModel(_userSkillService, _mediaService, _mediaSettings, _generalSettings, _skillSettings, _followService, _likeService, _commentService);
            return RespondSuccess(new { SkillData = model });
        }

        [HttpGet]
        [Route("{id:int}/users/get")]
        public IHttpActionResult GetSkillUsers(int id, int page)
        {
            var skill = _skillService.Get(id);
            if (skill == null)
                return NotFound();

            var userSkills = _userSkillService.Get(x => x.SkillId == id, page: page, count: _skillSettings.NumberOfUsersPerPageOnSinglePage,earlyLoad: x => x.User).ToList();
            var model =
                userSkills.Select(
                    x =>
                        x.ToModel(_mediaService, _mediaSettings, _generalSettings, firstMediaOnly: true,
                            withNextAndPreviousMedia: true, withSocialInfo: false));

            return RespondSuccess(new
            {
                UserSkills = model
            });
        }

        [HttpPost]
        [Authorize]
        [Route("post")]
        public IHttpActionResult Post(UserSkillEntityModel model)
        {
            if (!ModelState.IsValid)
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

            var mediaIds = model.MediaId?.ToList() ?? new List<int>();
            //get all medias
            var medias = _mediaService.Get(x => mediaIds.Contains(x.Id) && x.UserId == currentUser.Id).ToList();

           
            //get skill, 1.) by id 2.) by name 3.) create new otherwise
            var skill = _skillService.Get(model.Id) ??
                        (_skillService.FirstOrDefault(x => x.Name == model.SkillName) ?? new Skill() {
                            DisplayOrder = model.DisplayOrder,
                            UserId = currentUser.Id,
                            Name = model.SkillName
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
                    skill.Name = model.SkillName;
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
                foreach(var media in medias)
                    _mediaService.AttachMediaToEntity(userSkill, media);
                return RespondSuccess(new {
                    Skill = userSkill.ToModel(_mediaService, _mediaSettings, _generalSettings)
                });
            }
            return RespondSuccess(new {
                Skill = skill.ToModel()
            });
        }

        [HttpPost]
        [Authorize]
        [Route("media/post")]
        public IHttpActionResult Post(UserSkillEntityMediaModel model)
        {
            if (model.MediaId == 0 || model.UserSkillId == 0)
                return BadRequest();

            var currentUser = ApplicationContext.Current.CurrentUser;
            //check if user skill exists and that media exists
            var userSkill =
                _userSkillService.FirstOrDefault(x => x.Id == model.UserSkillId && x.UserId == currentUser.Id);

            if (userSkill == null)
                return NotFound();

            var media = _mediaService.Get(model.MediaId);
            if (media == null || media.UserId != currentUser.Id)
                return BadRequest();

            //attach media
            _mediaService.AttachMediaToEntity(userSkill, media);

            return RespondSuccess(new {
                MediaType = media.MediaType
            });
        }

        [HttpPost]
        [Authorize]
        [Route("featured-media")]
        public IHttpActionResult Post(SetFeaturedMediaModel requestModel)
        {
            var skillId = requestModel.SkillId;
            var mediaId = requestModel.MediaId;
            var currentUser = ApplicationContext.Current.CurrentUser;
           //check if the skill and media actually exist?
            var skill = _skillService.Get(skillId);
            if (skill == null)
                return NotFound();

            var canUpdate = currentUser.IsAdministrator() || currentUser.Id == skill.UserId;

            if (!canUpdate)
                return Unauthorized();

            //check if media exist
            var media = _mediaService.Get(mediaId);
            if (media == null || (media.UserId != currentUser.Id && !currentUser.IsAdministrator()))
                return Unauthorized();

            //media should also be a picture to proceed further.
            //todo: support video covers as well
            if (media.MediaType != MediaType.Image)
                return BadRequest("Can't set media as featured image");

            skill.FeaturedImageId = mediaId;
            _skillService.Update(skill);
            return RespondSuccess();
        }

        [HttpDelete]
        [Authorize]
        [Route("user/media/delete/{userSkillId}/{mediaId}")]
        public IHttpActionResult DeleteMedia(int userSkillId, int mediaId)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            //check if the skill and media actually exist?
            var userSkill = _userSkillService.Get(userSkillId);
            if (userSkill == null)
                return NotFound();

            var canUpdate = currentUser.IsAdministrator() || currentUser.Id == userSkill.UserId;

            if (!canUpdate)
                return Unauthorized();

            //check if media exist
            var media = _mediaService.Get(mediaId);
            if (media == null || (media.UserId != currentUser.Id && !currentUser.IsAdministrator()))
                return Unauthorized();

            _mediaService.DetachMediaFromEntity(userSkill, media);

            //delete the media as well
            _mediaService.Delete(media);
            return RespondSuccess();
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