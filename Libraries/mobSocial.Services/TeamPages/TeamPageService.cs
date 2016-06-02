using mobSocial.Core.Data;
using mobSocial.Data.Entity.TeamPages;

namespace mobSocial.Services.TeamPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public class TeamPageService : MobSocialEntityService<TeamPage>, ITeamPageService
    {
        public TeamPageService(IDataRepository<TeamPage> dataRepository) : base(dataRepository)
        {
        }
    }

}

    

