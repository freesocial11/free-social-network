using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Users
{
    public class UserEntityModel: RootEntityModel
    {
        public UserEntityModel()
        {
            AvailableRoles = new List<dynamic>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Active { get; set; }

        public string Remarks { get; set; }

        public string LastLoginIpAddress { get; set; }

        public DateTime LastLoginDateUtc { get; set; }

        public DateTime LastLoginDateLocal { get; set; }

        public List<dynamic> AvailableRoles { get; set; }

        public List<int> RoleIds { get; set; }

    }
}