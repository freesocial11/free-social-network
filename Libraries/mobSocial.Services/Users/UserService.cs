using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;

namespace mobSocial.Services.Users
{
    public class UserService : MobSocialEntityService<User>, IUserService
    {
        private readonly IDataRepository<Role> _roleRepository;
        private readonly IDataRepository<UserRole> _userRoleRepository;

        public UserService(IDataRepository<User> dataRepository, IDataRepository<Role> roleRepository, IDataRepository<UserRole> userRoleRepository) : base(dataRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public User GetCompleteUser(int userId)
        {
            var user = Get(userId);
            if (user == null)
                return null;
            //user roles
            var userRoleQuery = _userRoleRepository.Get(x => x.UserId == userId, earlyLoad: x => x.Role);
            user.UserRoles = userRoleQuery.ToList();
            return user;
        }

        public User GetCompleteUser(string email)
        {
            var user = FirstOrDefault(x => x.Email == email);
            if (user == null)
                return null;
            //user roles
            var userId = user.Id;
            user.UserRoles = _userRoleRepository.Get(x => x.UserId == userId, earlyLoad: x => x.Role).ToList();
            return user;
        }

        public IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, int page, int count)
        {
            return Get(x => (x.FirstName.StartsWith(searchText) || x.LastName.StartsWith(searchText) || x.Email.StartsWith(searchText)), x => x.FirstName, true, page,
                count).ToList();
        }

        public IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, string[] restrictToRoles, int page, int count)
        {
            var roleIds = _roleRepository.Get(x => restrictToRoles.Contains(x.SystemName)).Select(x => x.Id).ToArray();
            return SearchUsers(searchText, excludeLoggedInUser, roleIds, page, count);
        }

        public IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, int[] restrictToRoles, int page, int count)
        {
            var query = Get(
                x =>
                    (x.FirstName.StartsWith(searchText) || x.LastName.StartsWith(searchText) || x.Email.StartsWith(searchText)) &&
                    restrictToRoles.Intersect(x.UserRoles.Select(y => y.RoleId)).Any(), x => x.FirstName, true, page,
                count);
            return query.ToList();
        }
    }
}