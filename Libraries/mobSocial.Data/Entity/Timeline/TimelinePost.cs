using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Timeline
{
    public class TimelinePost : BaseEntity
    {
        public int OwnerId { get; set; }

        public string OwnerEntityType { get; set; }

        public string PostTypeName { get; set; }

        public bool IsSponsored { get; set; }

        public string Message { get; set; }

        public string AdditionalAttributeValue { get; set; }

        public int LinkedToEntityId { get; set; }

        public string LinkedToEntityName { get; set; }

        public bool IsHidden { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
    public class TimelinePostMap: BaseEntityConfiguration<TimelinePost> { }
}
