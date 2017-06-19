using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.GeoServices
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }

        public string TwoLetterIsoCode { get; set; }

        public string ThreeLetterIsoCode { get; set; }

        public int DisplayOrder { get; set; }
    }

    public class CountryMap : BaseEntityConfiguration<Country> { }
}