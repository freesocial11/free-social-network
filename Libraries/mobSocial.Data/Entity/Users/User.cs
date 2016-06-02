using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Users
{
    public class User : BaseEntity, ISoftDeletable, IPermalinkSupported, IHasEntityProperties<User>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string UserName { get; set; }

        public Guid Guid { get; set; }

        [Required]
        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public PasswordFormat PasswordFormat { get; set; }

        public bool Active { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime LastLoginDate { get; set; }

        public bool IsSystemAccount { get; set; }

        public string Remarks { get; set; }

        public string LastLoginIpAddress { get; set; }

        public int ReferrerId { get; set; }

        public bool Deleted { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }
    }

    public class UserMap : BaseEntityConfiguration<User>
    {
        public UserMap()
        {
        }
    }
}