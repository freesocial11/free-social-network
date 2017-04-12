using System.ComponentModel.DataAnnotations;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Addresses
{
    public class EntityAddress : BaseEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int AddressId { get; set; }

        public bool IsDefault { get; set; }
    }

    public class EntityAddressMap : BaseEntityConfiguration<EntityAddress> { }
}