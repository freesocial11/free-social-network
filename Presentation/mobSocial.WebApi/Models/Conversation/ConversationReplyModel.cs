using System;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationReplyModel : RootModel
    {
        public int Id { get; set; }

        public string ReplyText { get; set; }

        public int UserId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreatedLocal { get; set; }

        public ReplyStatus ReplyStatus { get; set; }

        public DateTime ReplyStatusLastUpdated { get; set; }

        public int ConversationId { get; set; }
    }
}