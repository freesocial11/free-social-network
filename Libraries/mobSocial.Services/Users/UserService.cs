using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;

namespace mobSocial.Services.Users
{
    public class UserService : MobSocialEntityService<User>, IUserService
    {
        public UserService(IDataRepository<User> dataRepository) : base(dataRepository)
        {
        }

        public IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, int page, int count)
        {
            return Get(x => (x.FirstName.Contains(searchText) || x.LastName.Contains(searchText) || x.Email.Contains(searchText)) 
            && (!excludeLoggedInUser || x.IsRegistered()), x => x.FirstName, true, page,
                count).ToList();
        }
    }
}