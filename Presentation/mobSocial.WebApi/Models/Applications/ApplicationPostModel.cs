using System.ComponentModel.DataAnnotations;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Applications
{
    public class ApplicationPostModel : RootEntityModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ApplicationUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public string TermsUrl { get; set; }

        [Required]
        public string RedirectUrl { get; set; }

        [Required]
        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public string AllowedOrigin { get; set; }
    }
}