using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public class VideoBattleVideoService : MobSocialEntityService<VideoBattleVideo>, IVideoBattleVideoService
    {

        public VideoBattleVideoService(IDataRepository<VideoBattleVideo> videoBattleVideoRepository) :
            base(videoBattleVideoRepository)
        {
        }

        public VideoBattleVideo GetBattleVideo(int battleId, int participantId)
        {
            var battleVideo = Repository.Get(x => x.VideoBattleId == battleId && x.ParticipantId == participantId).FirstOrDefault();
            return battleVideo;
        }

        public IList<VideoBattleVideo> GetBattleVideos(int battleId)
        {
            return Repository.Get(x => x.VideoBattleId == battleId).ToList();
        }
    }
}
