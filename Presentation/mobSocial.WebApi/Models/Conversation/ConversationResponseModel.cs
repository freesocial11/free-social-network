using System.Collections.Generic;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationResponseModel
    {
        public int ConversationId { get; set; }

        public IList<UserResponseModel> Users { get; set; }

        public IList<ConversationReplyModel> ConversationReplies { get; set; }
    }
}