using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Skills;

namespace mobSocial.Services.Skills
{
    public interface ISkillService : IBaseEntityService<Skill>
    {
        IList<UserSkill> GetUserSkills(int userId);

        IList<Skill> GetAllSkills(out int total, string search = "", int page = 1, int count = 15);

        IList<Skill> SearchSkills(string searchText, int page = 1, int count = 15);
    }
}