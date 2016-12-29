using System.Collections.Generic;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationResponseModel
    {
        public int ConversationId { get; set; }

        public Dictionary<int, UserResponseModel> Users { get; set; }

        public List<ConversationReplyModel> ConversationReplies { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverType { get; set; }

        public int UserId { get; set; }

        public int TotalReplies { get; set; }
    }
}