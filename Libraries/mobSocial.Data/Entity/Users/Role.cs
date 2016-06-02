using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Users
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }

        public bool IsSystemRole { get; set; }

        public string SystemName { get; set; }

        public bool IsActive { get; set; }
    }

    public class RoleMap : BaseEntityConfiguration<Role> { }
}