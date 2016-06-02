using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Battles;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Battles
{
    public interface IVideoBattleService : IBaseEntityService<VideoBattle>
    {
        IList<VideoBattle> GetAll(int? challengerId, int? participantId, int? videoGenreId, BattleStatus? battleStatus, BattleParticipationType? battleType, bool? isSponsorshipSupported, string searchTerm, BattlesSortBy? battlesSortBy, SortOrder? sortOrder, out int totalPages, int page = 1, int count = 15);
        
        //TODO: Move to a separate file for scheduler
        void SetScheduledBattlesOpenOrClosed();
    }
}