using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GroupPages;

namespace mobSocial.Services.TeamPages
{
    public interface ITeamPageGroupMemberService : IBaseEntityService<GroupPageMember>
    {
        IList<GroupPageMember> GetGroupPageMembersForTeam(int teamId);

        IList<GroupPageMember> GetGroupPageMembers(int groupId);

        void DeleteGroupPageMember(int groupId, int memberId);
    }
}