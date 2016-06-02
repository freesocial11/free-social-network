using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public interface ICapabilityService : IBaseEntityService<Capability>
    {
        IList<Capability> GetByRole(int roleId);

        IList<Capability> GetByRolesConsolidated(int[] roleIds);
    }
}