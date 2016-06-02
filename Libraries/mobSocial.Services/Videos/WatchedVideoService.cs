using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Videos;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Videos
{
    public class WatchedVideoService: MobSocialEntityService<WatchedVideo>, IWatchedVideoService
    {
        private readonly IDataRepository<WatchedVideo> _repository;
 
        public WatchedVideoService(IDataRepository<WatchedVideo> repository) : base(repository)
        {
            _repository = repository;
        }

        public bool IsVideoWatched(int customerId, int videoId, VideoType videoType)
        {
            return GetWatchedVideos(videoId, customerId, videoType).Any();
        }

        public WatchedVideo GetWatchedVideo(int customerId, int videoId, VideoType videoType)
        {
            var watchedVideos = GetWatchedVideos(videoId, customerId, videoType);
            return watchedVideos.FirstOrDefault();
        }

        public int GetViewCounts(int videoId, VideoType videoType)
        {
            return GetWatchedVideos(videoId, null, videoType).Count;
        }

        public IList<WatchedVideo> GetWatchedVideos(int? videoId, int? customerId, VideoType? videoType)
        {
            var query = _repository.Get(x => true);

            if (videoId.HasValue)
            {
                query = query.Where(x => x.VideoId == videoId);
            }

            if (customerId.HasValue)
            {
                query = query.Where(x => x.CustomerId == customerId);
            }

            if (videoType.HasValue)
            {
                query = query.Where(x => x.VideoType == videoType);
            }

            return query.ToList();
        }
    }
}