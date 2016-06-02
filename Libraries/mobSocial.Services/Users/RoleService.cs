using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Exception;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public class RoleService : MobSocialEntityService<Role>, IRoleService
    {
        private readonly IDataRepository<UserRole> _userRoleDataRepository;
 
        public RoleService(IDataRepository<Role> dataRepository, 
            IDataRepository<UserRole> userRoleDataRepository) : base(dataRepository)
        {
            _userRoleDataRepository = userRoleDataRepository;
        }

        public void AssignRoleToUser(Role role, User user)
        {
            var isAlreadyAssigned = GetUserRoles(user).Any(x => x.Id == role.Id);
            if (isAlreadyAssigned)
                return;

            _userRoleDataRepository.Insert(new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id
            });
        }

        public void AssignRoleToUser(string roleName, User user)
        {
            var role =
                Repository.Get(
                    x => string.Compare(x.SystemName, roleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();

            if(role == null)
                throw new mobSocialException(string.Format("The role with name '{0}' can't be found", roleName));

            AssignRoleToUser(role, user);
        }

        public IList<Role> GetUserRoles(int userId)
        {
            var userRoles = _userRoleDataRepository.Get(x => x.UserId == userId).Select(x => x.Role).ToList();
            return userRoles;
        }

        public IList<Role> GetUserRoles(User user)
        {
            return GetUserRoles(user.Id);
        }
    }
}