using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Battles
{
    public interface IVideoBattleParticipantService: IBaseEntityService<VideoBattleParticipant>
    {
        VideoBattleParticipant GetVideoBattleParticipant(int battleId, int participantId);

        BattleParticipantStatus GetParticipationStatus(int battleId, int participantId);

        IList<VideoBattleParticipant> GetVideoBattleParticipants(int battleId, BattleParticipantStatus? participantStatus);
    }
}