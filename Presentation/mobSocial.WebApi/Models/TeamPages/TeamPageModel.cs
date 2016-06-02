using System.Collections.Generic;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPageModel
    {

        public TeamPageModel()
        {
            Groups = new List<TeamPageGroupModel>();
        }

        public List<TeamPageGroupModel> Groups { get; set; }

        public string TeamPictureUrl { get; set; }

        public string TeamName { get; set; }

        public string TeamDescription { get; set; }
    }
}