using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Media;

namespace mobSocial.WebApi.Models.BusinessPages
{
    public class BusinessPageModel : RootPageModel
    {

        public BusinessPageModel()
        {
            AddCouponModel = new BusinessPageCouponModel();
            Coupons = new List<BusinessPageCouponModel>();
        }
        
        public List<BusinessPageCouponModel> Coupons { get; set; }
        public BusinessPageCouponModel AddCouponModel { get; set; }
        public List<MediaReponseModel> Pictures { get; set; }

        public bool CanEdit { get; set; }
    }

   

    
}