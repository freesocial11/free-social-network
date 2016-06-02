using System;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Timeline;

namespace mobSocial.Services.Timeline
{
    public class TimelineAutoPublisher : ITimelineAutoPublisher
    {
        private readonly ITimelineService _timelineService;
        public TimelineAutoPublisher(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        public void Publish<T>(T entity, string postTypeName, int ownerId) where T : BaseEntity
        {
            //create new timeline post
            var post = new TimelinePost() {
                Message = string.Empty,
                AdditionalAttributeValue = string.Empty,
                PostTypeName = postTypeName,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                OwnerId = ownerId,
                IsSponsored = false,
                OwnerEntityType = TimelinePostOwnerTypeNames.Customer,
                LinkedToEntityName = typeof(T).Name,
                LinkedToEntityId = entity.Id,
                PublishDate = DateTime.UtcNow
            };
            //save the post
            _timelineService.Insert(post);
        }
    }
}
