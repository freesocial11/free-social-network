using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class BattleUploadModel : RootModel
    {
        public int BattleId { get; set; }

        public int ParticipantId { get; set; }

        public int EntityId { get; set; }
    }
}
