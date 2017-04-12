using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GroupPages;

namespace mobSocial.Services.TeamPages
{
    public class TeamPageGroupMemberService : BaseEntityService<GroupPageMember>, ITeamPageGroupMemberService
    {
        private readonly IDataRepository<GroupPage> _groupPageRepository;

        public TeamPageGroupMemberService(IDataRepository<GroupPageMember> repository, 
            IDataRepository<GroupPage> groupPageRepository) : base(repository)
        {
            this._groupPageRepository = groupPageRepository;
        }

        public IList<GroupPageMember> GetGroupPageMembersForTeam(int teamId)
        {
            //find all groups for this team
            var allGroupsId = _groupPageRepository.Get(x => x.Team.Id == teamId).Select(x => x.Id);
            return Repository.Get(x => allGroupsId.Contains(x.GroupPageId)).OrderBy(x => x.DisplayOrder).ToList();
        }

        public IList<GroupPageMember> GetGroupPageMembers(int groupId)
        {
            return Repository.Get(x => x.GroupPageId == groupId).OrderBy(x => x.DisplayOrder).ToList();
        }

        public void DeleteGroupPageMember(int groupId, int memberId)
        {
            var groupMember = Repository.Get(x => x.GroupPageId == groupId && x.CustomerId == memberId).FirstOrDefault();
            if (groupMember != null)
                Delete(groupMember);
        }
    }
}