#region Author Information
// ConversationReplyStatus.cs
// 
// (c) 2016 Apexol Technologies. All Rights Reserved.
// 
#endregion

using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Conversations
{
    public class ConversationReplyStatus : BaseEntity
    {
        public int ReplyId { get; set; }

        public virtual ConversationReply Reply { get; set; }

        public ReplyStatus ReplyStatus { get; set; }

        public DateTime LastUpdated { get; set; }

        public int UserId { get; set; }
    }

    public class ConversationReplyStatusMap: BaseEntityConfiguration<ConversationReplyStatus> { }
}