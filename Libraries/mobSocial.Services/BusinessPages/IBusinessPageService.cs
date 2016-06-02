using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.BusinessPages;

namespace mobSocial.Services.BusinessPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IBusinessPageService : IBaseEntityService<BusinessPage>
    {
        List<BusinessPage> Search(string nameKeyword, string city, int? stateProvinceId, int? countryId);
    }

}
