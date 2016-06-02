using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.BusinessPages;

namespace mobSocial.Services.BusinessPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface IBusinessPageCouponService : IBaseEntityService<BusinessPageCoupon>
    {
        List<BusinessPageCoupon> GetAll(int businessPageId);
    }

}
