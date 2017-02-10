using System.Linq;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Skills;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Skills;
using mobSocial.Services.Social;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.Skills;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class SkillExtensions
    {
        public static SkillModel ToModel(this Skill skill)
        {
            var model = new SkillModel()
            {
                DisplayOrder = skill.DisplayOrder,
                SkillName = skill.SkillName,
                Id = skill.Id,
                UserId = skill.UserId
            };
            return model;
        }

        public static UserSkillModel ToModel(this UserSkill userSkill, IMediaService mediaService, MediaSettings mediaSettings, GeneralSettings generalSettings, bool onlySkillData = false, bool firstMediaOnly = false, bool withNextAndPreviousMedia = false, bool withSocialInfo = false)
        {
            var entityMedias = mediaService.GetEntityMedia<UserSkill>(userSkill.Id, null, count: int.MaxValue).ToList();
            var model = new UserSkillModel()
            {
                DisplayOrder = userSkill.Skill.DisplayOrder,
                SkillName = userSkill.Skill.SkillName,
                UserSkillId = userSkill.Id,
                Id = userSkill.SkillId,
                User = onlySkillData ? null : userSkill.User.ToModel(mediaService, mediaSettings),
                Media =
                    entityMedias.Take(firstMediaOnly ? 1 : 15)
                        .ToList()
                        .Select(
                            x =>
                                x.ToModel<UserSkill>(userSkill.Id, mediaService, mediaSettings, generalSettings,
                                    withNextAndPreviousMedia: withNextAndPreviousMedia, withSocialInfo: withSocialInfo, avoidMediaTypeForNextAndPreviousMedia: true))
                        .ToList(),
                TotalMediaCount = entityMedias.Count,
                TotalPictureCount = entityMedias.Count(x => x.MediaType == MediaType.Image),
                TotalVideoCount = entityMedias.Count(x => x.MediaType == MediaType.Video),
                ExternalUrl = userSkill.ExternalUrl,
                Description = userSkill.Description,
                SeName = userSkill.Skill.GetPermalink().ToString()
            };
            return model;
        }

        public static SkillWithUsersModel ToSkillWithUsersModel(this Skill skill, IUserSkillService userSkillService, IMediaService mediaService,
            MediaSettings mediaSettings, GeneralSettings generalSettings, SkillSettings skillSettings, IFollowService followService, ILikeService likeService, ICommentService commentService)
        {
            var currentUser = ApplicationContext.Current.CurrentUser;
            var model = new SkillWithUsersModel()
            {
                Skill = skill.ToModel(),
                FeaturedMediaImageUrl = skill.FeaturedImageId > 0 ? mediaService.GetPictureUrl(skill.FeaturedImageId) : mediaSettings.DefaultSkillCoverUrl
            };

            var perPage = skillSettings.NumberOfUsersPerPageOnSinglePage;
            //by default we'll send data for 15 users. rest can be queried with paginated request
            //todo: make this thing configurable to set number of users to return with this response
            var userSkills = userSkillService.Get(x => x.SkillId == skill.Id, page: 1, count: perPage, earlyLoad: x => x.User).ToList();

            model.UserSkills =
                userSkills.Select(x => x.ToModel(mediaService, mediaSettings, generalSettings, false, true, true, false)).ToList();

            model.CurrentPage = 1;
            model.UsersPerPage = perPage;
            model.TotalUsers = userSkillService.Count(x => x.SkillId == skill.Id);
            model.FollowerCount = followService.GetFollowerCount<Skill>(skill.Id);

            //does this user follow this skill?
            var userFollow = followService.GetCustomerFollow<Skill>(currentUser.Id, skill.Id);
            model.CanFollow = currentUser.Id != skill.UserId;
            model.FollowStatus = userFollow == null ? 0 : 1;

            model.TotalComments = commentService.GetCommentsCount(skill.Id, "skill");
            model.LikeStatus = likeService.GetCustomerLike<Skill>(currentUser.Id, skill.Id) == null ? 0 : 1;
            model.TotalLikes = likeService.GetLikeCount<Skill>(skill.Id);
            return model;
        }
    }
}