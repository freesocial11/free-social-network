using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Timeline;

namespace mobSocial.Services.Timeline
{
    public class TimelineService : MobSocialEntityService<TimelinePost>, ITimelineService
    {
        public TimelineService(IDataRepository<TimelinePost> repository) : base(repository)
        {

        }

        public IList<TimelinePost> GetByEntityIds(string owerEntityType, int[] ownerEntityIds, bool onlyPublishable = true, int count = 15, int page = 1)
        {
            var query = Repository.Get(x => x.OwnerEntityType == owerEntityType && ownerEntityIds.Contains(x.OwnerId));
            if (onlyPublishable)
                query = query.Where(x => x.PublishDate <= DateTime.UtcNow);
            return query.OrderByDescending(x => x.DateCreated)
                .Skip(count*(page - 1))
                .Take(count).ToList();
        }
    }
}
