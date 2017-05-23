using mobSocial.Core.Data;
using mobSocial.Data.Database.Attributes;

namespace mobSocial.Data.Entity.Users
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }

        public bool IsSystemRole { get; set; }

        public string SystemName { get; set; }

        public bool IsActive { get; set; }
    }

    [ToRunTimeView("mobSocial_RoleView")]
    public class RoleMap : BaseEntityConfiguration<Role> { }
}