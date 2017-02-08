using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Skills
{
    public class UserSkillEntityModel : RootEntityModel
    {
        [Required]
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }

        public string Description { get; set; }

        public string ExternalUrl { get; set; }

        public int[] MediaId { get; set; }

        public int UserSkillId { get; set; }

        public bool SystemSkill { get; set; }
    }
}