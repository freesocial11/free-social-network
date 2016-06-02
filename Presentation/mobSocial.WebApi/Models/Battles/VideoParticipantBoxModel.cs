using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoParticipantBoxModel : RootModel
    {
        public VideoParticipantPublicModel VideoParticipant { get; set; }

        public VideoBattlePublicModel VideoBattle { get; set; }
    }
}
