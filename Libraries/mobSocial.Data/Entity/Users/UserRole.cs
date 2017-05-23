using mobSocial.Core.Data;
using mobSocial.Data.Database.Attributes;

namespace mobSocial.Data.Entity.Users
{
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }

    [ToRunTimeView("mobSocial_UserRoleView")]
    public class UserRoleMap : BaseEntityConfiguration<UserRole> { }
}