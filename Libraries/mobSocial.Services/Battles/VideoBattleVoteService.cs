using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public class VideoBattleVoteService : MobSocialEntityService<VideoBattleVote>, IVideoBattleVoteService
    {
      

        public IList<VideoBattleVote> GetVideoBattleVotes(int videoBattleId, int? userId)
        {
            if (userId.HasValue)
            {
                return Repository.Get(x => x.VideoBattleId == videoBattleId && x.UserId == userId.Value).ToList();

            }
            return Repository.Get(x => x.VideoBattleId == videoBattleId).ToList();
        }


        public VideoBattleVoteService(IDataRepository<VideoBattleVote> dataRepository) : base(dataRepository)
        {
        }

       
    }
}
