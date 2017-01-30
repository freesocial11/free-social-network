using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Media;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Skills
{
    public class UserSkillModel : RootEntityModel
    {
        public SkillModel Skill { get; set; }

        public string Description { get; set; }

        public UserResponseModel User { get; set; }      

        public MediaReponseModel Media { get; set; }

        public string ExternalUrl { get; set; }
    }
}