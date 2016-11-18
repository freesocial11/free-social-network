using System;
using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Timeline
{
    public class TimelinePostModel : RootEntityModel
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

        public IList<TagModel> InlineTags { get; set; }

        public class TagModel
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }
    }
}
