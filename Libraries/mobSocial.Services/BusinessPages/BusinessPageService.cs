using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.BusinessPages;

namespace mobSocial.Services.BusinessPages
{
    public class BusinessPageService : MobSocialEntityService<BusinessPage>, IBusinessPageService
    {

        public BusinessPageService(IDataRepository<BusinessPage> dataRepository) : base(dataRepository)
        {
        }

        public List<BusinessPage> Search(string nameKeyword, string city, int? stateProvinceId, int? countryId)
        {
            var searchQuery = Repository.Get(x => true);

            if(!string.IsNullOrEmpty(nameKeyword))
               searchQuery = searchQuery.Where(x => x.Name.ToLower().Contains(nameKeyword));

            if (!string.IsNullOrEmpty(city))
                searchQuery = searchQuery.Where(x => x.City.ToLower().Contains(city));

            if (stateProvinceId.HasValue)
                searchQuery = searchQuery.Where(x => x.StateProvinceId == stateProvinceId);

            if(countryId.HasValue)
                searchQuery = searchQuery.Where(x => x.CountryId == countryId);

            var searchResults = searchQuery.ToList();

            return searchResults;
        }

        
    }

}
