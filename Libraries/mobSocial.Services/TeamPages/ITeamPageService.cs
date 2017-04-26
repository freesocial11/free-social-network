using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.TeamPages;

namespace mobSocial.Services.TeamPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public interface ITeamPageService : IBaseEntityService<TeamPage>
    {
        TeamPage GetTeamPageByGroup(int groupId);

        /// <summary>
        /// Safely deletes a team after deleting the groups and member associations
        /// </summary>
        void SafeDelete(TeamPage team);

        /// <summary>
        /// Gets team pages by the ower
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        IList<TeamPage> GetTeamPagesByOwner(int ownerId);
    }

}
