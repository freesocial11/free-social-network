using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.GroupPages;
using mobSocial.Data.Entity.TeamPages;

namespace mobSocial.Services.TeamPages
{
    /// <summary>
    /// Product service
    /// </summary>
    public class TeamPageService : MobSocialEntityService<TeamPage>, ITeamPageService
    {
        private readonly IDataRepository<GroupPage> _groupPageRepository;
        private readonly IDataRepository<GroupPageMember> _groupPageMembeRepository;


        public TeamPageService(IDataRepository<TeamPage> dataRepository, IDataRepository<GroupPage> groupPageRepository, IDataRepository<GroupPageMember> groupPageMembeRepository) : base(dataRepository)
        {
            _groupPageRepository = groupPageRepository;
            _groupPageMembeRepository = groupPageMembeRepository;
        }

        public TeamPage GetTeamPageByGroup(int groupId)
        {
            //first let's query the team id of the group
            var group = _groupPageRepository.Get(x => x.Id == groupId).FirstOrDefault();
            //query the team page
            return @group == null ? null : Get(@group.TeamPageId, x => x.GroupPages.Select(y => y.Members));

        }

        public void SafeDelete(TeamPage team)
        {
            var groupPageIds = team.GroupPages.Select(x => x.Id);
            //get group member associations
            var groupMembers = _groupPageMembeRepository.Get(x => groupPageIds.Contains(x.GroupPageId)).ToList();

            //delete all group members

            while (groupMembers.Any())
                _groupPageMembeRepository.Delete(groupMembers.First());

            while (team.GroupPages.Any())
                //delete all groups
                _groupPageRepository.Delete(team.GroupPages.First());

            //delete the team
            Delete(team);

        }

        public IList<TeamPage> GetTeamPagesByOwner(int ownerId)
        {
            return Repository.Get(x => x.CreatedBy == ownerId).ToList();
        }
    }

}

    

