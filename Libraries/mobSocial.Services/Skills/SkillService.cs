using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Skills;

namespace mobSocial.Services.Skills
{
    public class SkillService : BaseEntityService<Skill>, ISkillService
    {
        private readonly IDataRepository<UserSkill> _userSkillDataRepository;

        public SkillService(IDataRepository<Skill> dataRepository, IDataRepository<UserSkill> userSkillDataRepository) : base(dataRepository)
        {
            _userSkillDataRepository = userSkillDataRepository;
        }

        public IList<UserSkill> GetUserSkills(int userId)
        {
            //get user's skill ids
            return _userSkillDataRepository.Get(x => x.UserId == userId, earlyLoad: x => x.Skill).ToList();
        }

        public IList<Skill> GetAllSkills(out int total, string search = "", int page = 1, int count = 15)
        {
            var q = Repository.Get(x => search == "" || x.SkillName.StartsWith(search));
            total = q.Count(); //total records
            return
                q.OrderBy(x => x.SkillName)
                    .Skip(count*(page - 1))
                    .Take(count)
                    .ToList();
        }

        public IList<Skill> SearchSkills(string searchText, int page = 1, int count = 15)
        {
            searchText = searchText.ToLower();
            return
               Repository.Get(x => x.SkillName.ToLower().StartsWith(searchText))
                   .OrderBy(x => x.DisplayOrder)
                   .AsEnumerable()
                   .Distinct(new SkillComparer())
                   .Skip((page - 1) * count)
                   .Take(count)
                   .ToList();
        }

        
    }

    public class SkillComparer : IEqualityComparer<Skill>
    {
        public bool Equals(Skill x, Skill y)
        {
            return x.SkillName == y.SkillName;
        }

        public int GetHashCode(Skill obj)
        {
            return obj.SkillName.GetHashCode();
        }
    }
}