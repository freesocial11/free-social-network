using mobSocial.Core.Data;
using mobSocial.Data.Entity.Videos;

namespace mobSocial.Services.Videos
{
    public class VideoGenreService : MobSocialEntityService<VideoGenre>, IVideoGenreService
    {
        public VideoGenreService(IDataRepository<VideoGenre> dataRepository) : base(dataRepository)
        {
        }
    }
}
