using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Addresses
{
    /// <summary>
    /// Addresses associated with various entities such as User, Page etc.
    /// </summary>
    public class Address : BaseEntity
    {
        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public int StateProvinceId { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipPostalCode { get; set; }

        public int CountryId { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }
    }
}