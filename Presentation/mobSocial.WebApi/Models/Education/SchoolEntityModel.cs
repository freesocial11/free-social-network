using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Education
{
    public class SchoolEntityModel : RootEntityModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        public int LogoId { get; set; }
    }
}