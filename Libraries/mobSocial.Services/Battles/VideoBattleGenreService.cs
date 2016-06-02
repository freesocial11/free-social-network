using mobSocial.Core.Data;
using mobSocial.Data.Entity.Battles;

namespace mobSocial.Services.Battles
{
    public class VideoBattleGenreService : MobSocialEntityService<VideoBattleGenre>, IVideoBattleGenreService
    {
        public VideoBattleGenreService(IDataRepository<VideoBattleGenre> dataRepository) : base(dataRepository)
        {
        }
    }
}
