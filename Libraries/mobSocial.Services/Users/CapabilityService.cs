using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Services.Users
{
    public class CapabilityService : MobSocialEntityService<Capability>, ICapabilityService
    {
        private readonly IDataRepository<RoleCapability> _roleCapabilityRepository;
        public CapabilityService(IDataRepository<Capability> dataRepository, 
            IDataRepository<RoleCapability> roleCapabilityRepository) : base(dataRepository)
        {
            _roleCapabilityRepository = roleCapabilityRepository;
        }

        public IList<Capability> GetByRole(int roleId)
        {
            var distinctCapabilityIds = _roleCapabilityRepository.Get(x => x.RoleId == roleId).Select(x => x.CapabilityId).Distinct();
            return Repository.Get(x => distinctCapabilityIds.Contains(x.Id)).ToList();
        }

        public IList<Capability> GetByRolesConsolidated(int[] roleIds)
        {
            var distinctCapabilityIds = _roleCapabilityRepository.Get(x => roleIds.Contains(x.RoleId)).Select(x => x.CapabilityId).Distinct();
            return Repository.Get(x => distinctCapabilityIds.Contains(x.Id)).ToList();
        }
    }
}