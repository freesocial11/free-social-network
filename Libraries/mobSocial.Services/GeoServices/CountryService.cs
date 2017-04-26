using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GeoServices;

namespace mobSocial.Services.GeoServices
{
    public class CountryService : BaseEntityService<Country>, ICountryService
    {
        public CountryService(IDataRepository<Country> dataRepository) : base(dataRepository) { }
    }
}