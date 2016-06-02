using System.Collections.Generic;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Timeline;

namespace mobSocial.Services.Timeline
{
    public interface ITimelineService : IBaseEntityService<TimelinePost>
    {
        IList<TimelinePost> GetByEntityIds(string owerEntityType, int[] ownerEntityIds, bool onlyPublishable = true, int count = 15, int page = 1);


    }
}