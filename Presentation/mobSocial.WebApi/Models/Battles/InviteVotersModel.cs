using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class InviteVotersModel : RootModel
    {
        public int VideoBattleId { get; set; }

        public IList<int> VoterIds { get; set; }

        public IList<string> Emails { get; set; } 
    }
}
