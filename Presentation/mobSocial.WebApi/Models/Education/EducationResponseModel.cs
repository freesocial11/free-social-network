using System;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Education
{
    public class EducationResponseModel : RootModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public SchoolResponseModel School { get; set; }

        public EducationType EducationType { get; set; }
    }
}