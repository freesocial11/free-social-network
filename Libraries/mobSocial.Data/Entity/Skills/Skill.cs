using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Skills
{
    public class Skill : BaseEntity
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }
    }

    public class SkillMap : BaseEntityConfiguration<Skill>
    {
    }
}
