namespace mobSocial.Data.Entity.Addresses
{
    public class EntityAddress
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int AddressId { get; set; }

        public bool IsDefault { get; set; }
    }
}