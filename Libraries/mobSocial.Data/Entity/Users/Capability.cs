using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Users
{
    public partial class Capability : BaseEntity
    {
        public string CapabilityName { get; set; }

        public bool IsActive { get; set; }
    }

    public class CapabilityMap : BaseEntityConfiguration<Capability> { }
}