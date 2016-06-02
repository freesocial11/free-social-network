using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.BusinessPages;

namespace mobSocial.Services.BusinessPages
{
    public class BusinessPageCouponService : MobSocialEntityService<BusinessPageCoupon>, IBusinessPageCouponService
    {
        public BusinessPageCouponService(IDataRepository<BusinessPageCoupon> dataRepository) : base(dataRepository)
        {
        }

        public List<BusinessPageCoupon> GetAll(int businessPageId)
        {
            return Repository.Get(x => x.BusinessPageId == businessPageId).ToList();
        }
    }

}
