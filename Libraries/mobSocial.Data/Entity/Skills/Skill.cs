using mobSocial.Core.Data;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Skills
{
    public class Skill : BaseEntity, IPermalinkSupported
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserId { get; set; }

        public int FeaturedImageId { get; set; }

        public string Name { get; set; }
    }

    public class SkillMap : BaseEntityConfiguration<Skill>
    {
    }
}
