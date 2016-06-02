using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class UpdateParticipantStatusModel : RootModel
    {
        public int BattleId { get; set; }

        public BattleParticipantStatus VideoBattleParticipantStatus { get; set; }

        public int ParticipantId { get; set; }

        public string Remarks { get; set; }
    }
}
