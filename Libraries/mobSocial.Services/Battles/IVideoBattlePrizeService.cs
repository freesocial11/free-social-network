using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public interface IVideoBattlePrizeService : IBaseEntityService<VideoBattlePrize>
    {
        IList<VideoBattlePrize> GetBattlePrizes(int videoBattleId);
    }
}
