using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public class VideBattlePrizeService: MobSocialEntityService<VideoBattlePrize>, IVideoBattlePrizeService
    {
 
        public VideBattlePrizeService(IDataRepository<VideoBattlePrize> repository) : base(repository)
        {
        }
        public IList<VideoBattlePrize> GetBattlePrizes(int videoBattleId)
        {
            return Repository.Get(x => x.VideoBattleId == videoBattleId).OrderBy(x => x.WinnerPosition).ToList();
        }
    }
}
