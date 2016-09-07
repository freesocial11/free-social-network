using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Education
{
    public class Education : BaseEntity, IUserResource
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int UserId { get; set; }

        public int SchoolId { get; set; }

        public virtual School School { get; set; }

        public EducationType EducationType { get; set; }
    }

    public class EducationMap: BaseEntityConfiguration<Education> { }
}
