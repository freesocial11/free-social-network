using System;
using System.Web.Mvc;

namespace mobSocial.WebApi.Configuration.Mvc.Models
{
    public class RootPageModel : RootEntityModel
    {

        #region Properties
        public string Name { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipPostalCode { get; set; }
        public int CountryId { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        [AllowHtml]
        public string Description { get; set; }

        [AllowHtml]
        public string MetaKeywords { get; set; }

        [AllowHtml]
        public string MetaDescription { get; set; }

        [AllowHtml]
        public string MetaTitle { get; set; }

        [AllowHtml]
        public string SeName { get; set; }

        public string MainPictureUrl { get; set; }

        public string FullSizeImageUrl { get; set; }
        #endregion
    }
}