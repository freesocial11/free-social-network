using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Social
{
    public class Friend : BaseEntity
    {
        public int FromCustomerId { get; set; }
        public int ToCustomerId { get; set; }
        public bool Confirmed { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public int NotificationCount { get; set; }
        public DateTime? LastNotificationDate { get; set; }

        public Friend()
        {
            NotificationCount = 0;
        }

    }

    public class FriendMap : BaseEntityConfiguration<Friend> { }


}