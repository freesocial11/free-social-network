using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class InviteParticipantsModel : RootModel
    {
        public int VideoBattleId { get; set; }

        public IList<int> ParticipantIds { get; set; }

        public IList<string> Emails { get; set; } 
    }
}
