using System.Collections.Generic;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{

    public class RoleCapabilityService : IRoleCapabilityService
    {
        private readonly ICapabilityService _capabilityService;
 
        public RoleCapabilityService(ICapabilityService capabilityService)
        {
            _capabilityService = capabilityService;
        }

        public IList<Capability> GetRoleCapabilities(string roleSystemName)
        {
            throw new System.NotImplementedException();
        }

        public IList<Capability> GetRoleCapabilities(int roleId)
        {
            return _capabilityService.GetByRole(roleId);
        }

        public IList<Capability> GetConsolidatedCapabilities(int[] roleIds)
        {
            return _capabilityService.GetByRolesConsolidated(roleIds);
        }

        public IList<Capability> GetConsolidatedCapabilities(string[] roleSystemNames)
        {
            throw new System.NotImplementedException();
        }
    }
}