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
        public UserService(IDataRepository<User> dataRepository, IDataRepository<Role> roleRepository) : base(dataRepository)
        {
            _roleRepository = roleRepository;
        }

        public IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, int page, int count)
        {
            return Get(x => (x.FirstName.Contains(searchText) || x.LastName.Contains(searchText) || x.Email.Contains(searchText)), x => x.FirstName, true, page,
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
                    (x.FirstName.Contains(searchText) || x.LastName.Contains(searchText) || x.Email.Contains(searchText)) &&
                    restrictToRoles.Intersect(x.UserRoles.Select(y => y.RoleId)).Any(), x => x.FirstName, true, page,
                count);
            return query.ToList();
        }
    }
}