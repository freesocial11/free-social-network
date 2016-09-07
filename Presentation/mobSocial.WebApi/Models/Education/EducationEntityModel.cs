using System;
using System.ComponentModel.DataAnnotations;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Education
{
    public class EducationEntityModel : RootEntityModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [Required]
        public int SchoolId { get; set; }

        public EducationType EducationType { get; set; }
    }
}