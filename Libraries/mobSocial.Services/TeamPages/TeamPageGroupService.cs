using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GroupPages;

namespace mobSocial.Services.TeamPages
{
    public class TeamPageGroupService : BaseEntityService<GroupPage>, ITeamPageGroupService
    {
        private readonly IDataRepository<GroupPageMember> _memberRepository;

        public TeamPageGroupService(IDataRepository<GroupPage> repository, IDataRepository<GroupPageMember> memberRepository) : base(repository)
        {
            this._memberRepository = memberRepository;
        }

       public IList<GroupPage> GetGroupPagesByTeamId(int teamId)
        {
            return Repository.Get(x => x.Team.Id == teamId).OrderBy(x => x.DisplayOrder).ToList();
        }

        public void SafeDelete(GroupPage groupPage)
        {
            while(groupPage.Members.Any())
                _memberRepository.Delete(groupPage.Members.First());

            Delete(groupPage);
        }
    }
}
