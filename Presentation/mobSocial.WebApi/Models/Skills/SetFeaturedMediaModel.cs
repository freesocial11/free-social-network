using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Skills
{
    public class SetFeaturedMediaModel : RootModel
    {
        public int SkillId { get; set; }

        public int MediaId { get; set; }
    }
}