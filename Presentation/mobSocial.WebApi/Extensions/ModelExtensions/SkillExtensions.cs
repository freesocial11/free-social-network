using System.Linq;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Skills;
using mobSocial.Data.Enum;
using mobSocial.Services.Extensions;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Skills;
using mobSocial.Services.Social;
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
            var model = new UserSkillModel()
            {
                DisplayOrder = userSkill.Skill.DisplayOrder,
                SkillName = userSkill.Skill.SkillName,
                UserSkillId = userSkill.Id,
                Id = userSkill.SkillId,
                User = onlySkillData ? null : userSkill.User.ToModel(mediaService, mediaSettings),
                Media =
                    mediaService.GetEntityMedia<UserSkill>(userSkill.Id, null, count: firstMediaOnly ? 1 : 15)
                        .ToList()
                        .Select(
                            x =>
                                x.ToModel<UserSkill>(userSkill.Id, mediaService, mediaSettings, generalSettings,
                                    withNextAndPreviousMedia: withNextAndPreviousMedia, withSocialInfo: withSocialInfo, avoidMediaTypeForNextAndPreviousMedia: true))
                        .ToList(),
                ExternalUrl = userSkill.ExternalUrl,
                Description = userSkill.Description,
                SeName = userSkill.Skill.GetPermalink().ToString()
            };
            return model;
        }

        public static SkillWithUsersModel ToSkillWithUsersModel(this Skill skill, IUserSkillService userSkillService, IMediaService mediaService,
            MediaSettings mediaSettings, GeneralSettings generalSettings, IFollowService followService)
        {
            var model = new SkillWithUsersModel()
            {
                Skill = skill.ToModel(),
                FeaturedMediaImageUrl = skill.FeaturedImageId > 0 ? mediaService.GetPictureUrl(skill.FeaturedImageId) : mediaSettings.DefaultSkillCoverUrl
            };

            var perPage = 15;
            //by default we'll send data for 15 users. rest can be queried with paginated request
            //todo: make this thing configurable to set number of users to return with this response
            var userSkills = userSkillService.Get(x => x.SkillId == skill.Id, page: 1, count: perPage, earlyLoad: x => x.User).ToList();

            model.UserSkills =
                userSkills.Select(x => x.ToModel(mediaService, mediaSettings, generalSettings, false, true, true, false)).ToList();

            model.CurrentPage = 1;
            model.UsersPerPage = perPage;
            model.TotalUsers = userSkillService.Count(x => x.SkillId == skill.Id);
            model.FollowerCount = followService.GetFollowerCount<Skill>(skill.Id);
            return model;
        }
    }
}