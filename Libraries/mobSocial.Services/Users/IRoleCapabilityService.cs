using System.Collections.Generic;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public interface IRoleCapabilityService
    {
        IList<Capability> GetRoleCapabilities(string roleSystemName);

        IList<Capability> GetRoleCapabilities(int roleId);

        IList<Capability> GetConsolidatedCapabilities(int[] roleIds);

        IList<Capability> GetConsolidatedCapabilities(string[] roleSystemNames);

    }
}