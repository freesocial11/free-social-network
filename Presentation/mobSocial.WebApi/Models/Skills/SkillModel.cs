using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Skills
{
    public class SkillModel : RootEntityModel
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }
    }
}
