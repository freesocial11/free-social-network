using System.Web.Mvc;

namespace mobSocial.WebApi.Models.BusinessPages
{
    public class BusinessPageCouponModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        [AllowHtml]
        public string AdditionalInformation { get; set; }
        public string Country { get; set; }
        public int DisplayOrder { get; set; }
        public int BusinessPageId { get; set; }
        public string Disclaimer { get; set; }
    }

}