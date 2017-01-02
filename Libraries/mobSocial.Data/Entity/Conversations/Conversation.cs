using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Conversations
{
    public class Conversation : BaseEntity, IHasEntityProperties<Conversation>, ISoftDeletable
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual IList<ConversationReply> ConversationReplies { get; set; }

        public bool Deleted { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverType { get; set; }

        public bool ReceiverDeleted { get; set; }

        public bool SenderDeleted { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class ConversationMap : BaseEntityConfiguration<Conversation> { }
}