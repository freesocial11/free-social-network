using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.GroupPages;

namespace mobSocial.Services.TeamPages
{
    public interface ITeamPageGroupService : IBaseEntityService<GroupPage>
    {
        IList<GroupPage> GetGroupPagesByTeamId(int teamId);

        /// <summary>
        /// Safely deletes a group page after deleting all the member associations
        /// </summary>
        /// <param name="groupPage"></param>
        void SafeDelete(GroupPage groupPage);
    }
}