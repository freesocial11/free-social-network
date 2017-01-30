using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Skills;

namespace mobSocial.Services.Skills
{
    public class UserSkillService : BaseEntityService<UserSkill>, IUserSkillService
    {
        public UserSkillService(IDataRepository<UserSkill> dataRepository) : base(dataRepository) {}
    }
}