using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public interface IVideoBattleVideoService: IBaseEntityService<VideoBattleVideo>
    {
        VideoBattleVideo GetBattleVideo(int battleId, int participantId);

        IList<VideoBattleVideo> GetBattleVideos(int battleId);
    }
}