using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public interface IRoleService : IBaseEntityService<Role>
    {
        void AssignRoleToUser(Role role, User user);

        void AssignRoleToUser(string roleName, User user);

        IList<Role> GetUserRoles(int userId);

        IList<Role> GetUserRoles(User user);

    }
}