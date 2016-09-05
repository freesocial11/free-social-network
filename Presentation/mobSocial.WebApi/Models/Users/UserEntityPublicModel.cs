using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class UserEntityPublicModel: RootEntityModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string UserName { get; set; }

        public string ProfileImageUrl { get; set; }

        public int ProfileImageId { get; set; }

        public string CoverImageUrl { get; set; }

        public int CoverImageId { get; set; }

    }
}