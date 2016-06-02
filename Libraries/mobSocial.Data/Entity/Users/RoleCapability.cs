using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Users
{
    public class RoleCapability : BaseEntity
    {
        public int RoleId { get; set; }

        public int CapabilityId { get; set; }

        public virtual Role Role { get; set; }

        public virtual Capability Capability { get; set; }
    }

    public class RoleCapabilityMap : BaseEntityConfiguration<RoleCapability> { }
}