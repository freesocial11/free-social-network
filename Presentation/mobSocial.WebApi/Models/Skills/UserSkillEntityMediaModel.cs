using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Skills
{
    public class UserSkillEntityMediaModel : RootModel
    {
        public int UserSkillId { get; set; }

        public int MediaId { get; set; }
    }
}