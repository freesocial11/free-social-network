using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Entity.Conversations
{
    public class ConversationReply : BaseEntity
    {
        public int ConversationId { get; set; }

        public virtual Conversation Conversation { get; set; }

        public string ReplyText { get; set; }

        public int UserId { get; set; }   

        public virtual User User { get; set; }

        public DateTime DateCreated { get; set; }

        public string IpAddress { get; set; }

        public virtual IList<ConversationReplyStatus> ConversationReplyStatus { get; set; }
    }

    public class ConversationReplyMap : BaseEntityConfiguration<ConversationReply>
    {
        public ConversationReplyMap()
        {
            HasRequired(x => x.User).WithMany().WillCascadeOnDelete(false);
        }
    }
}