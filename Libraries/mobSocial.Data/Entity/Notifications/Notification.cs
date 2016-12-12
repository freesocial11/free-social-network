using System;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Entity.Notifications
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public bool IsRead { get; set; }

        public DateTime PublishDateTime { get; set; }

        public DateTime? ReadDateTime { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int? NotificationEventId { get; set; }

        public virtual NotificationEvent NotificationEvent { get; set; }

        public int InitiatorId { get; set; }

        public string InitiatorName { get; set; }
    }

    public class NotificationMap : BaseEntityConfiguration<Notification>
    {
        public NotificationMap()
        {
            HasOptional(x => x.NotificationEvent)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.NotificationEventId);
        }
    }
}