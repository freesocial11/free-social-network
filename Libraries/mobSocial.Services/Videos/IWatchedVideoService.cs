using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Videos;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Videos
{
    public interface IWatchedVideoService : IBaseEntityService<WatchedVideo>
    {
        bool IsVideoWatched(int customerId, int videoId, VideoType videoType);

        WatchedVideo GetWatchedVideo(int customerId, int videoId, VideoType videoType);

        int GetViewCounts(int videoId, VideoType videoType);

        IList<WatchedVideo> GetWatchedVideos(int? videoId, int? customerId, VideoType? videoType);
    }
}