using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.BusinessPages
{
    public class BusinessPage : BaseEntity, IPermalinkSupported
    {


        public string Name { get; set; }
        public int CountryId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateProvinceId { get; set; }
        public string ZipPostalCode { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }


        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public virtual List<BusinessPageCoupon> Coupons { get; set; }
    }
}