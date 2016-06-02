using System.Collections.Generic;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPageGroupModel
    {

        public TeamPageGroupModel()
        {
            Members = new List<TeamPageGroupMemberModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<TeamPageGroupMemberModel> Members { get; set; }

    }
}