using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Media;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Skills
{
    public class UserSkillModel : RootEntityModel
    {
        public string SkillName { get; set; }

        public int DisplayOrder { get; set; }

        public int UserSkillId { get; set; }

        public string Description { get; set; }

        public UserResponseModel User { get; set; }      

        public IList<MediaReponseModel> Media { get; set; }

        public string ExternalUrl { get; set; }

        public string FeaturedImageUrl { get; set; }

        public string SeName { get; set; }
    }
}