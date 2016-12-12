using System;

namespace mobSocial.WebApi.Models.Notifications
{
    public class NotificationModel
    {
        public bool IsRead { get; set; }

        public DateTime? PublishDateTime { get; set; }

        public DateTime? ReadDateTime { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string EntityTitle { get; set; }

        public string EntityDescription { get; set; }

        public string EventName { get; set; }

        public int InitiatorId { get; set; }

        public string InitiatorDisplayName { get; set; }

        public string InitiatorImageUrl { get; set; }
    }
}