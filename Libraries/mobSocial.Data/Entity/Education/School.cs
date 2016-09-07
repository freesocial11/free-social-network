using mobSocial.Core.Data;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Education
{
    public class School : BaseEntity, IUserResource
    {
        public string Name { get; set; }

        public string City { get; set; }

        public int LogoId { get; set; }

        public int UserId { get; set; }
    }

    public class SchoolMap : BaseEntityConfiguration<School> { }
}
