using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public interface IUserService : IBaseEntityService<User>
    {
        IList<User> SearchUsers(string searchText, bool excludeLoggedInUser, int page, int count);
    }
}