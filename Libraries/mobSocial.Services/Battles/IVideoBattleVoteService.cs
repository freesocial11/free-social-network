using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public interface IVideoBattleVoteService: IBaseEntityService<VideoBattleVote>
    {
        IList<VideoBattleVote> GetVideoBattleVotes(int videoBattleId, int? userId);
    }
}