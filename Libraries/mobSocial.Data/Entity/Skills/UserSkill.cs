using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Entity.Skills
{
    public class UserSkill : BaseEntity
    {
        public int SkillId { get; set; }

        public virtual Skill Skill { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string ExternalUrl { get; set; }

        public int DisplayOrder { get; set; }

        public string Description { get; set; }

    }

    public class UserSkillMap : BaseEntityConfiguration<UserSkill>
    {
    }
}