using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.GeoServices
{
    public class State : BaseEntity
    {
        public int CountryId { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public int DisplayOrder { get; set; }
    }
}