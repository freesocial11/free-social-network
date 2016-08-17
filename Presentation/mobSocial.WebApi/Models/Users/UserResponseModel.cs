using System;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class UserResponseModel : RootEntityModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string ProfileImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string ProfileUrl { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreatedLocal { get; set; }

        public bool Active { get; set; }

        public DateTime? LastLoginDateUtc { get; set; }

        public DateTime? LastLoginDateLocal { get; set; }

    }
}